import"./esm-chunk-d81494b9.js";import"./esm-chunk-a2731447.js";import{changeDom as t,ensureElement as e,getParentByClassName as n,addClassNameToElement as o,subscribeElementContentSize as i,RequestAnimationFrame as s,elementIsInDOM as r}from"./esm-dom-utils-0e4190ff.js";import{a as c,d as l}from"./esm-chunk-9c16a801.js";import{d as u,r as a}from"./esm-chunk-f1e43abb.js";import{T as d}from"./esm-chunk-1b6abd73.js";import{K as h}from"./esm-chunk-808bf349.js";import{scrollToFocusedItem as f}from"./esm-grid-84cea811.js";import"./esm-grid-column-resize-81963b84.js";import"./esm-dx-style-helper-e7dc0266.js";import{show as m,getElementPaddingRight as p,getScrollbarWidth as g}from"./esm-popup-utils-f634e322.js";import{initFocusHidingEvents as w}from"./esm-focus-utils-b9e104a7.js";import{E as v}from"./esm-chunk-d4fc448c.js";import{getClientRectWithMargins as y,getClientRect as b,PointBlz as C,geometry as x}from"./esm-dragAndDropUnit-06605dbe.js";const E=document.body,S=new window.WeakMap,T=new Map,P={subtree:!0,childList:!0},D=new window.MutationObserver((function(t){t.forEach(M)}));function M(t){t.removedNodes.forEach(O)}function O(t){const e=T.get(t);T.delete(t)&&(0===T.size&&D.disconnect(),e())}class R{constructor(t,e){this.element=t,this.getClientRect=e}get leftTopCorner(){const t=this;return new L(this.element,(function(e){return t.getClientRect(e)}),(function(t){return{x:0,y:0}}))}get leftBottomCorner(){const t=this;return new L(this.element,(function(e){const n=t.getClientRect(e);return new C(n.x,n.bottom)}),(function(e){const n=t.getClientRect(e);return new C(0,-n.height)}))}get rightTopCorner(){const t=this;return new L(this.element,(function(e){const n=t.getClientRect(e);return new C(n.right,n.y)}),(function(e){const n=t.getClientRect(e);return new C(-n.width,0)}))}get rightBottomCorner(){const t=this;return new L(this.element,(function(e){const n=t.getClientRect(e);return new C(n.right,n.bottom)}),(function(e){const n=t.getClientRect(e);return new C(-n.width,-n.height)}))}get center(){const t=this;return new L(this.element,(function(e){return t.getClientRect(e).center}))}}class L{constructor(t,e,n){this.element=t,this.getLocation=e,this.getDelta=n}get location(){return this.getLocation(this.element)}get delta(){return this.getDelta(this.element)}anchorTo(t){return new N(this,t)}}class N{constructor(t,e){this.point=t,this.anchor=e,this.events=new v(this);const n=[];if(n.push([window,"resize"]),n.push([window,"scroll"]),this.containers=function(t,e){const n=[];for(;null!==t&&"BODY"!==t.tagName&&"#document"!==t.nodeName;)e(t)&&n.push(t),t=t.parentNode;return 0===n.length?null:n}(this.anchor.element.parentNode,this.isElementScrollable),this.containers&&this.containers.forEach((function(t){n.push([t,"scroll"])})),this.checkInCasesInt(n),"undefined"!=typeof ResizeObserver){const t=this;this.resizeObserver=new window.ResizeObserver((function(){t.update()})),this.resizeObserver.observe(this.anchor.element),this.resizeObserver.observe(this.point.element)}else this.resizeObserver=null;this.notStaticParent=this.point.element.offsetParent,this.notStaticParent=null!==this.notStaticParent?this.notStaticParent:{x:0,y:0,scrollTop:0,scrollLeft:0},this.notStaticParent=this.isStatic(this.notStaticParent)?window:this.notStaticParent,this.update()}isElementScrollable(t){const e=window.getComputedStyle(t);return"static"===e.position&&("scroll"===e["overflow-x"]||"scroll"===e["overflow-y"]||"auto"===e["overflow-x"]||"auto"===e["overflow-y"])}isStatic(t){if(!t.style)return!1;return"static"===window.getComputedStyle(t).position}update(){const e=this.notStaticParent===window?{x:window.scrollX,y:window.scrollY}:{x:this.notStaticParent.scrollLeft,y:this.notStaticParent.scrollTop},n=x(this.anchor.location,"+",this.point.delta,"-",this.notStaticParent,"+",e),o=this.point.element;t((function(){o.style.left=n.x+"px",o.style.top=n.y+"px"}))}checkInCasesInt(t){const e=this.events;t.forEach((function(t){e.attachEvent(t[0],t[1],(function(t){this.update()}))}))}checkInCases(){return this.containers?(this.checkInCasesInt(Array.from(arguments)),this.update(),this):this}dispose(){this.events&&(this.events.dispose(),this.events=null,this.dropDownStartPos=null,this.containers=null,this.resizeObserver&&this.resizeObserver.disconnect())}}function k(t){return new R(t,y)}function B(t){return new R(t,b)}const I={Popup:0,Modal:1},z=window.console,H=1,j=2,W=new WeakMap;let F;function _(t){return t.compareDocumentPosition(document.body)&window.Node.DOCUMENT_POSITION_DISCONNECTED}function q(t){let n=(t=e(t)).querySelector("div.dxbs-dm.dropdown-menu");return n||(n=t.querySelector("div.dxgvCSD.dxbs-grid-vsd")),n}function A(t,n,o,i){return new Promise((function(o,s){let r=q(t);return r=e(r),r?(0===i&&(r.style.minWidth=t.offsetWidth+"px"),2===i&&(r.style.width=t.offsetWidth+"px"),Y(t,r,n),f(r),o()):o()}))}function Y(e,n,o,i){W.has(e)&&(W.get(e).disconnect(),W.delete(e));const s=n.offsetParent;if(!s)return t((function(){n.style.visibility=""})),void 0;const r=s.getBoundingClientRect(),c=e.getBoundingClientRect(),l=c.top-r.top,u=r.bottom-c.bottom;let a;const d=window.getComputedStyle(n),h=n.offsetHeight+Math.max(parseFloat(d.marginTop),0)+Math.max(parseFloat(d.marginBottom),0)+8;switch(o){case j:a=!0,r.top+(l-h)<=0&&r.top+l+e.offsetHeight+h+window.pageYOffset<=Math.max(document.body.scrollHeight,window.innerHeight)&&(a=!1);break;case H:default:a=!1,r.bottom-(u-h)>=window.innerHeight&&r.top+l-h+window.pageYOffset>=0&&(a=!0)}const f=c.left+n.offsetWidth+8>=document.body.clientWidth;t((function(){F&&F.dispose(),F=a?f?k(n).rightBottomCorner.anchorTo(B(e).rightTopCorner):k(n).leftBottomCorner.anchorTo(B(e).leftTopCorner):f?k(n).rightTopCorner.anchorTo(B(e).rightBottomCorner):k(n).leftTopCorner.anchorTo(B(e).leftBottomCorner),n.style.visibility=""}))}const U=[{value:0,text:""},{value:1,text:"above"},{value:2,text:"below"},{value:4,text:"top-sides"},{value:8,text:"bottom-sides"},{value:16,text:"outside-left"},{value:32,text:"outside-right"},{value:64,text:"left-sides"},{value:128,text:"right-sides"}];function V(t,e,n){let o=t.target;for(;o;){if(o===e)return!1;o=o.parentElement}n&&n()}function G(t){return"hidden"!==t.style.visibility||t.classList.contains("visually-hidden")}const K="a[href], input:not([disabled]), button:not([disabled]), [tabindex]:not([tabindex='-1'])";function X(t,e){if(n(t.srcElement,"modal-header"))return!0;const o=n(t.srcElement,"column-chooser-elements-container"),i=n(o,"modal-body");if(!o||!i)return!1;if(i.clientHeight>=o.clientHeight)return!0;const s=t.touches[0].pageY-e.touches[0].pageY,r=function(t){return t.getBoundingClientRect().top},c=o.querySelector(".column-chooser-element-container");if(c&&r(c)===r(i)&&s>=0&&t.cancelable)return!0;const l=function(t){return Math.round(t.getBoundingClientRect().bottom)},u=o.querySelector(".column-chooser-element-container:last-child");return!!(u&&l(u)===l(i)&&s<=0&&t.cancelable)||void 0}function J(t,e,o){if(o!==I.Modal)return;const i=t.getBoundingClientRect(),s=e.getBoundingClientRect(),r=n(t,"dxbs-gridview"),c=r&&r.querySelector("thead");if(!c)return;const l=c.getBoundingClientRect(),u=(l||i).bottom;return u>.5*e.clientHeight?u-i.top<.5*e.clientHeight?u-s.top-.5*e.clientHeight:i.top-s.top-2:void 0}function Q(t){const e=t.getElementsByClassName("modal-body")[0];e.style.width=e.offsetWidth+"px",e.style.height=e.offsetHeight+"px"}const Z={init:function(t,n,o,i){if(t=e(t),n=e(n),o=e(o),t){if(u(t),o){const e=function(e){return V(e,t,(function(){r(t)||u(t);const e=document.activeElement===n,s=o&&G(o);(e||s)&&i.invokeMethodAsync("OnDropDownLostFocus",n.value).catch(t=>z.error(t))}))};c(document,d.touchMouseDownEventName,e),a(t,(function(){l(document,d.touchMouseDownEventName,e)}))}return Promise.resolve("ok")}},dispose:function(t){return(t=e(t))&&(clearTimeout(t._closeTimerId),u(t)),F&&F.dispose(),Promise.resolve("ok")},showAdaptiveDropdown:function(r,c,l,u,a,f){r=e(r);const v=n(r,l);if(!v)return;const y=document.documentElement,b=document.documentElement.style.overflow,C=document.body.style.overflow,x=document.body.scroll,M=c.dialogType,O=c.alignment,R=M===I.Modal;M===I.Popup?0===O?Y(v,r,c.dropDownDirection):m(r,v,function(t){let e="";return U.forEach(n=>{0!=(n.value&t)&&(e&&(e+=" "),e+=n.text)}),e}(O)):(r.style.paddingRight=p(r)+g()+"px",document.body.style.paddingRight=parseFloat(p(document.body))+g(),document.body.classList.add("modal-open"));let L=!1;function N(t){(!r.contains(t.srcElement)||M===I.Modal&&r===t.srcElement)&&B(r)}function k(t){y.removeEventListener("focusin",k),null===t.relatedTarget&&t.target&&r&&r.contains(t.target)&&t.target.focus()}function B(t){if(!L){if(L=!0,_(t))return;t._closeTimerId=setTimeout((function(){H(),a.invokeMethodAsync("CloseDialog").catch((function(e){_(t)||z.error(e)}))}),200)}}function H(){y.removeEventListener(d.touchMouseDownEventName,N),window.removeEventListener("resize",W),F(M,!1),r=null}function j(){const t=r.querySelector(K);t&&t.focus()}function W(){if(!n(r,"modal-dialog-owner"))return;const t=r.firstElementChild.classList,e=J(v,y,M),o=y.clientHeight>y.clientWidth?"topVertical":"topHorizontal";s((function(){t.contains("topVertical")||t.contains("topHorizontal")?t.remove("transition"):t.add("transition"),e&&(y.scrollTop=e),t.remove("topVertical","topHorizontal"),t.add(o)}))}function F(t,e){if(t!==I.Modal)return;let n,o,i;e?(n="hidden",o="hidden",i="no"):(n=b,o=C,i=x),document.documentElement.style.overflow=n,document.body.style.overflow=o,document.body.scroll=i}return y.addEventListener(d.touchMouseDownEventName,N),r.addEventListener("keydown",(function(t){t.keyCode===h.Esc&&B(r)})),r.addEventListener("focusout",(function(t){L||(null===t.relatedTarget||r.contains(t.relatedTarget)?null===t.relatedTarget&&y.addEventListener("focusin",k):!function(t){const e=r.compareDocumentPosition(t);e&window.Node.DOCUMENT_POSITION_PRECEDING?!function(){const t=function(t){const e=t.querySelectorAll(K);return e[e.length-1]}(r);t&&t.focus()}():e&window.Node.DOCUMENT_POSITION_FOLLOWING&&j()}(t.relatedTarget))})),function(t,e,n,o){t&&e.addEventListener("touchmove",t=>{t.srcElement===e&&t.preventDefault()});if(o===I.Modal){let t=null;n.addEventListener("touchstart",e=>{t=e}),n.addEventListener("touchmove",e=>{X(e,t)&&e.preventDefault(),t=e})}}(R,r,v,M),function(t){if(S.has(t))return S.get(t);const e=new Promise((function(e){0===T.size&&D.observe(E,P);T.set(t,()=>{t=null,e()})}));return S.set(t,e),e}(r).then(()=>{M===I.Modal&&(document.body.classList.remove("modal-open"),document.body.style.paddingRight=parseFloat(p(document.body))-g()),H(),r=null}),F(M,!0),M===I.Popup?t((function(){r&&(c.showFocus||(o(r,"dxbs-focus-hidden"),w(r)),j(),function(t,e){const o=n(t,e);if(t&&o){const e=n(t,"dx-menu-bar");e&&e.classList.contains("vertical")||(i(o,s),s())}function s(){const e=t.getBoundingClientRect(),i=o.getBoundingClientRect(),s=parseFloat(t.style["margin-left"]);if(s&&(e.x=e.x-s),e.x+e.width>i.x+i.width&&i.width>e.width){const o=n(t.parentNode,"dropdown-menu");if(o){const n=o.getBoundingClientRect(),i=parseFloat(window.getComputedStyle(o,null).getPropertyValue("border-right-width"));t.style["margin-left"]="-"+(e.x+e.width-n.x-i)+"px"}else t.style["margin-left"]="-"+(e.x+e.width-i.x-i.width)+"px"}else s&&(t.style["margin-left"]="")}}(r,f))})):(window.addEventListener("resize",W),W()),Promise.resolve()},updateGridDropDown:A,setInlineModalSize:Q};export default Z;export{I as DialogType,q as getDropDownElement,G as isDropDownVisible,V as onOutsideClick,J as scrollDropDown,Q as setInlineModalSize,Y as setPositionOfDropDown,X as shouldPreventTouchMove,A as updateGridDropDown};
