import{D as e}from"./dom-a06f5987.js";import{E as t}from"./evt-20927eae.js";import{e as i,x as n,R as s,y as o}from"./dom-utils-417838ed.js";import{k as l}from"./key-643beca5.js";import{ensureAccentColorStyle as h}from"./dx-style-helper-89412acc.js";import"./touch-2ca1b361.js";function r(e){window.dxAccessibilityHelper||(window.dxAccessibilityHelper=new a),window.dxAccessibilityHelper.sendMessageToAssistiveTechnology(e)}class a{constructor(){this._helperElement=null}get helperElement(){return null==this._helperElement&&(this._helperElement=this.createHelperElement()),this._helperElement}createHelperElement(){const e=document.createElement("DIV");return e.className="dxAIFE dxAIFME",e.setAttribute("role","note"),e.setAttribute("aria-live","assertive"),document.documentElement.appendChild(e),e}sendMessageToAssistiveTechnology(e){this.helperElement.innerHTML=e,setTimeout((()=>{this.helperElement.innerHTML=""}),300)}}var d,m;!function(e){e[e.Disabled=0]="Disabled",e[e.NextColumn=1]="NextColumn",e[e.Component=2]="Component"}(d||(d={})),function(e){e[e.Default=0]="Default",e[e.Minimization=1]="Minimization",e[e.Maximization=2]="Maximization"}(m||(m={}));const u=30;class c{constructor(e,t,i,n){this._mainElement=e,this._nextHeaderCellElement=e.nextElementSibling,this._blazorComponent=n,this._gridResizeProxy=t,this._mode=i,this._resizeParameters={allowResize:!1,minWidth:-1,currentWidth:-1,maxWidth:-1},this._resizeAnchor=null,this._lastWidth=-1,this._lastNextWidth=-1,this._timeoutId=null,this._leftMouseXBoundary=-1,this._rightMouseXBoundary=-1,this._initWidth=-1,this._initNextWidth=-1,this._allWidth=-1,this._totalDiffX=0}get mainElement(){return this._mainElement}get nextHeaderCellElement(){return this._nextHeaderCellElement}get hasNextColumn(){return!!this._nextHeaderCellElement}get hasColumnId(){return this.mainElement.hasAttribute("data-dxdg-column-id")}get gridResizeProxy(){return this._gridResizeProxy}get blazorComponent(){return this._blazorComponent}get resizeAnchor(){return this._resizeAnchor}get caption(){return this.mainElement.innerText}get isResizeAllowed(){return this._mode!==d.Disabled&&this.hasColumnId&&(this.hasNextColumn||this._mode===d.Component)}get step(){let e=Math.ceil((this._resizeParameters.maxWidth-this._resizeParameters.minWidth)/100);return e>5&&(e=5),e}initialize(){this.createResizeAnchor(),R().initializeHeaderCellEvents(this),this.initializeEvents()}initializeEvents(){this.isResizeAllowed&&(this.mainElement.addEventListener("focus",this.onFocus.bind(this)),this.mainElement.addEventListener("keydown",this.onKeyDown.bind(this)),this.mainElement.addEventListener("keyup",this.onKeyUp.bind(this)))}removeEvents(){this.mainElement.removeEventListener("focus",this.onFocus.bind(this)),this.mainElement.removeEventListener("keydown",this.onKeyDown.bind(this)),this.mainElement.removeEventListener("keyup",this.onKeyUp.bind(this))}onFocus(e){t.getEventSource(e)===this.mainElement&&(this.onFocusCore(),this.updateResizeParameters())}onKeyDown(e){if(t.getEventSource(e)===this.mainElement)switch(e.keyCode){case l.KeyCode.Left:this.onKeyResize(-this.step),e.stopPropagation(),e.preventDefault();break;case l.KeyCode.Right:this.onKeyResize(this.step),e.stopPropagation(),e.preventDefault()}}onKeyResize(e){-1===this._lastWidth&&this.onStartKeyResize();let t=1500;this.hasNextColumn&&(t=this._lastWidth+this._lastNextWidth);const i=this._lastWidth+e;i>30&&(this._totalDiffX+=e),s((()=>{this.setWidth(i,t,this._totalDiffX)}).bind(this)),this._timeoutId&&clearTimeout(this._timeoutId)}onStartKeyResize(){this._lastWidth=this.mainElement.getBoundingClientRect().width,this._lastNextWidth=this.hasNextColumn?this.nextHeaderCellElement.getBoundingClientRect().width:0,this.gridResizeProxy.initializeComponentsWidths(),this._totalDiffX=0}onKeyUp(e){if(t.getEventSource(e)!==this.mainElement)return;const i=e.keyCode;i!==l.KeyCode.Left&&i!==l.KeyCode.Right||(this._timeoutId&&clearTimeout(this._timeoutId),this._timeoutId=setTimeout((()=>this.invokeSetWidth()),500))}updateResizeParameters(){const e=this.mainElement.getBoundingClientRect().width,t=this.hasNextColumn?this.nextHeaderCellElement.getBoundingClientRect().width:0,i=this.isResizeAllowed;let n=1500;this._mode===d.NextColumn&&(n=e+t-30),this._resizeParameters={allowResize:i,minWidth:i?30:e,currentWidth:e,maxWidth:i?n:e}}update(e){if(!this._resizeAnchor)return;const t=this.isResizeAllowed;this._nextHeaderCellElement=this.mainElement.nextElementSibling,this._mode=e,this.isResizeAllowed?t||(this.mainElement.appendChild(this._resizeAnchor),R().initializeHeaderCellEvents(this)):t&&(this.mainElement.removeChild(this._resizeAnchor),R().removeHeaderCellEvents(this))}createResizeAnchor(){const t=document.createElement("div");e.addClassName(t,"dxColumnResizeAnchor"),t.dxResizableHeaderCell=this,this._resizeAnchor=t,this.mainElement.appendChild(t)}onMouseDown(){this.updateState()}updateState(){this.setMouseBoundaries(),this.onFocusCore(),this.gridResizeProxy.initializeComponentsWidths()}setMouseBoundaries(){const e=this.mainElement.getBoundingClientRect(),t=Math.min(30,e.width);if(this._leftMouseXBoundary=e.left+t-10,this._initWidth=e.width,this._mode===d.NextColumn){const t=this.nextHeaderCellElement.getBoundingClientRect();this._rightMouseXBoundary=e.right+t.width-30,this._initNextWidth=t.width,this._allWidth=e.width+t.width}}removeMouseBoundaries(){this._leftMouseXBoundary=-1,this._rightMouseXBoundary=-1,this._initWidth=-1,this._initNextWidth=-1,this._allWidth=-1}getResizeType(e){return-1===this._leftMouseXBoundary?m.Default:e<=this._leftMouseXBoundary?m.Minimization:-1!==this._rightMouseXBoundary&&e>=this._rightMouseXBoundary?m.Maximization:m.Default}onFocusCore(){this.isResizeAllowed&&(this.ensureWidthSyncronized(),r(`Width is ${this.mainElement.offsetWidth} pixels. Use arrow keys to resize.`))}onDragResizeAnchor(e,t){if(!this.isResizeAllowed)return;const i=this.getResizeType(t),n=this.getNewWidth(e,i);i===m.Minimization&&(e=n-this._initWidth),this.setWidth(n,this._allWidth,e)}getNewWidth(e,t){switch(t){case m.Minimization:{const e=this.mainElement.getBoundingClientRect().width;return Math.min(30,e)}case m.Maximization:return this._allWidth-30;default:return this._initWidth+e}}onDragResizeAnchorStop(){this.removeMouseBoundaries(),this.invokeSetWidth()}setWidth(e,t,i){if(!t||!this.isWidthChanged(e)||!this.isValidWidth(e,t,i))return!1;const n=(s=this.mainElement,"border-box"===window.getComputedStyle(s,null).getPropertyValue("box-sizing")?0:o(s));var s;const l=t-(e-=n)-n;return this._lastWidth=e,this._lastNextWidth=l,this.gridResizeProxy.setWidth(this._mode,e,l,i),!0}isWidthChanged(e){return e!==this._lastWidth}async invokeSetWidth(){-1!==this._lastWidth&&-1!==this._lastNextWidth&&(this.gridResizeProxy.resetWidth(),R().lockResize(),await this.blazorComponent.invokeMethodAsync("SetColumnWidths",g(this.mainElement),this._lastWidth,g(this.nextHeaderCellElement),this._lastNextWidth),this._lastWidth=-1,this._lastNextWidth=-1,R().unlockResize())}isValidWidth(e,t,i){let n=e>=30||(-1!==this._lastWidth?e>this._lastWidth:i>0);return n&&this._mode!==d.Component&&(n=e<=t-30),n}async ensureWidthSyncronized(){if(this.isWidthSyncronized())return;const e=this.gridResizeProxy,t=await this.blazorComponent.invokeMethodAsync("GetCellCache");e.synchronizeWidth(t)}isWidthSyncronized(){let e=!1;const t=this.gridResizeProxy.getColElementInlineWidth();if(0===t)e=!0;else{e=t===this.mainElement.getBoundingClientRect().width}return e}}class _{constructor(e,t){this.mainElement=e,this._resizeElements=t,this._colElements=null,this._elementsToSetComponentWidth=null,this._elementsToSyncWidth=null,this._elementsToCheckScroll=null,this._elementsToReset=null,this._initWidths=new Map,this._hasHorizontalScroll=null}get colElements(){return w(this._colElements,this._resizeElements.colElements)}get elementsToSetComponentWidth(){return w(this._elementsToSetComponentWidth,this._resizeElements.elementsToSetComponentWidth)}get elementsToSyncWidth(){return w(this._elementsToSyncWidth,this._resizeElements.elementsToSyncWidth)}get elementsToCheckScroll(){return w(this._elementsToCheckScroll,this._resizeElements.elementsToCheckScroll)}get elementsToReset(){return w(this._elementsToReset,this._resizeElements.elementsToReset)}update(e){this._resizeElements=e,this._colElements=null,this._elementsToSetComponentWidth=null,this._elementsToSyncWidth=null,this._elementsToCheckScroll=null,this._elementsToReset=null}setWidth(e,t,i,n){this.colElements&&(r(`${t} pixels`),this.setComponentWidth(n),this.colElements.forEach((n=>{if(n.style.width=f(t),e===d.NextColumn){n.nextElementSibling.style.width=f(i)}})))}setComponentWidth(e){this.elementsToSetComponentWidth&&(this.isScrollExists()?this._hasHorizontalScroll=!0:this.setComponentWidthCore(e))}setComponentWidthCore(e){var t;this._hasHorizontalScroll&&this.onHorizontalScrollDisappears();const i=[],n=this._initWidths;null===(t=this.elementsToSetComponentWidth)||void 0===t||t.forEach((t=>i.push(new z(t,(n.get(t)||0)+e)))),this.elementsToSyncWidth&&i.push(new z(this.elementsToSyncWidth[0],(n.get(this.elementsToSyncWidth[1])||0)+e)),i.forEach((e=>e.apply()))}onHorizontalScrollDisappears(){this._hasHorizontalScroll=!1,this.resetWidth(),R().updateState()}initializeComponentsWidths(){var e;if(!this.elementsToSetComponentWidth)return;const t=new Map;if(null===(e=this.elementsToSetComponentWidth)||void 0===e||e.forEach((e=>t.set(e,e.getBoundingClientRect().width))),this.elementsToSyncWidth){const e=this.elementsToSyncWidth[1];t.set(e,e.getBoundingClientRect().width)}this._initWidths=t}isScrollExists(){if(!1===this._hasHorizontalScroll)return!1;const e=this.elementsToCheckScroll;if(!e)return!1;return e[0].clientWidth<e[1].clientWidth}getColElementInlineWidth(){let e=0;const t=this.colElements;for(let i=0;t&&i<t.length;i++){const n=t[i].style.width;if(""!==n){e=parseInt(n);break}}return e}synchronizeWidth(e){s((function(){const t=e.map((e=>{const t=i(e[0]);return t?{width:t.getBoundingClientRect().width,cols:x(e[1])}:null})).filter((e=>e));for(let e=0;e<t.length;e++){const i=t[e].cols;for(let n=0;n<i.length;n++)i[n].style.width=f(t[e].width)}}))}resetWidth(){this.elementsToReset&&!this.isScrollExists()&&this.elementsToReset.forEach((e=>{e.style.width=""}))}}class z{constructor(e,t){this._element=e,this._width=t}apply(){this._element.style.width=f(this._width)}}class p{constructor(){this._currentPageX=-1,this._pageX=-1,this._resizeLock=0,this._resizableHeaderCell=null}get pageX(){return this._pageX}get resizableHeaderCell(){return this._resizableHeaderCell}initializeHeaderCellEvents(e){e.resizeAnchor&&this.initializeMouseDown(e.resizeAnchor)}initializeMouseDown(e){e.addEventListener("pointerdown",C)}initializeMouseMove(){document.addEventListener("pointermove",W)}initializeMouseUp(){document.addEventListener("pointerup",y)}removeHeaderCellEvents(e){const t=e.resizeAnchor;t&&t.removeEventListener("pointerdown",C)}onMouseDown(e,t){this._resizableHeaderCell=e,e.onMouseDown(),this._pageX=t,this.initializeMouseMove(),this.initializeMouseUp()}lockResize(){this._resizeLock++}unlockResize(){this._resizeLock--}isResizeLocked(){return this._resizeLock>0}setCurrentPageX(e){this._currentPageX=e}updateState(){var e;this._pageX=this._currentPageX,null===(e=this._resizableHeaderCell)||void 0===e||e.updateState()}invalidateState(){this._pageX=-1,this._resizableHeaderCell=null}}function g(e){if(!e)return(-1).toString();const t=e.getAttribute("data-dxdg-column-id");return(t?t.split("|"):[])[0]}function E(e){return!1!==e.isPrimary?e.pageX:0}function C(e){const i=t.getEventSource(e);if(!i)return;const n=R(),s=i.dxResizableHeaderCell;if(s&&s.isResizeAllowed){const t=E(e);n.onMouseDown(s,t)}}function W(e){const t=R();if(t.isResizeLocked()||-1===t.pageX)return;const i=E(e);t.setCurrentPageX(i);const n=i-t.pageX;if(0!==n){const e=t.resizableHeaderCell;e&&s((function(){e.onDragResizeAnchor(n,i)}))}}function y(){const e=R();e.resizableHeaderCell&&e.resizableHeaderCell.onDragResizeAnchorStop(),e.invalidateState(),document.removeEventListener("pointermove",W),document.removeEventListener("pointerup",y)}function f(e){return e+"px"}function w(e,t){return t?(e||(e=x(t)),e):null}function x(e){return e.map((e=>document.getElementById(e)))}function R(){return null===window.dxResizeEventManagerInstance&&(window.dxResizeEventManagerInstance=new p),window.dxResizeEventManagerInstance}window.dxResizeEventManagerInstance=null;const v=new Map;function S(e,t,s,o){if(!(e=i(e)))return Promise.reject("failed");let l=v.get(e);if(l)l.update(s),l.gridResizeProxy.update(t);else{const i=new _(e,t);l=new c(e,i,s,o),l.isResizeAllowed&&(l.initialize(),v.set(e,l))}return h(),n(e,(function(){l.removeEvents(),v.delete(e)})),Promise.resolve("ok")}const M={initResizeColumn:S};export default M;export{d as ColumnResizeMode,S as initResizeColumn,u as minColumnWidth};
