DxBlazorInternal.define("cjs-dropdowns-0377dcb5.js",(function(e,t,n){e("./cjs-chunk-c5286524.js"),e("./cjs-chunk-0da7e9be.js");var o=e("./cjs-dom-utils-393f2f58.js"),i=e("./cjs-chunk-ad620f3e.js"),r=e("./cjs-chunk-843046a9.js"),s=e("./cjs-chunk-96905969.js"),c=e("./cjs-chunk-e9e6b6d6.js"),a=e("./cjs-grid-d812d2f2.js");e("./cjs-grid-column-resize-b7516f73.js"),e("./cjs-dx-style-helper-616b781d.js");var l=e("./cjs-popup-utils-a5804500.js"),u=e("./cjs-focus-utils-0ac5f035.js"),d=e("./cjs-chunk-b8f9edc9.js"),h=e("./cjs-dragAndDropUnit-7f87e559.js"),f=document.body,m=new window.WeakMap,g=new Map,p={subtree:!0,childList:!0},v=new window.MutationObserver((function(e){e.forEach(w)}));function w(e){e.removedNodes.forEach(y)}function y(e){var t=g.get(e);g.delete(e)&&(0===g.size&&v.disconnect(),t())}var b=function(e,t){this.element=e,this.getClientRect=t},C={leftTopCorner:{configurable:!0},leftBottomCorner:{configurable:!0},rightTopCorner:{configurable:!0},rightBottomCorner:{configurable:!0},center:{configurable:!0}};C.leftTopCorner.get=function(){var e=this;return new E(this.element,(function(t){return e.getClientRect(t)}),(function(e){return{x:0,y:0}}))},C.leftBottomCorner.get=function(){var e=this;return new E(this.element,(function(t){var n=e.getClientRect(t);return new h.PointBlz(n.x,n.bottom)}),(function(t){var n=e.getClientRect(t);return new h.PointBlz(0,-n.height)}))},C.rightTopCorner.get=function(){var e=this;return new E(this.element,(function(t){var n=e.getClientRect(t);return new h.PointBlz(n.right,n.y)}),(function(t){var n=e.getClientRect(t);return new h.PointBlz(-n.width,0)}))},C.rightBottomCorner.get=function(){var e=this;return new E(this.element,(function(t){var n=e.getClientRect(t);return new h.PointBlz(n.right,n.bottom)}),(function(t){var n=e.getClientRect(t);return new h.PointBlz(-n.width,-n.height)}))},C.center.get=function(){var e=this;return new E(this.element,(function(t){return e.getClientRect(t).center}))},Object.defineProperties(b.prototype,C);var E=function(e,t,n){this.element=e,this.getLocation=t,this.getDelta=n},j={location:{configurable:!0},delta:{configurable:!0}};j.location.get=function(){return this.getLocation(this.element)},j.delta.get=function(){return this.getDelta(this.element)},E.prototype.anchorTo=function(e){return new P(this,e)},Object.defineProperties(E.prototype,j);var P=function(e,t){this.point=e,this.anchor=t,this.events=new d.EventRegister(this);var n=[];if(n.push([window,"resize"]),n.push([window,"scroll"]),this.containers=function(e,t){for(var n=[];null!==e&&"BODY"!==e.tagName&&"#document"!==e.nodeName;)t(e)&&n.push(e),e=e.parentNode;return 0===n.length?null:n}(this.anchor.element.parentNode,this.isElementScrollable),this.containers&&this.containers.forEach((function(e){n.push([e,"scroll"])})),this.checkInCasesInt(n),"undefined"!=typeof ResizeObserver){var o=this;this.resizeObserver=new window.ResizeObserver((function(){o.update()})),this.resizeObserver.observe(this.anchor.element),this.resizeObserver.observe(this.point.element)}else this.resizeObserver=null;this.notStaticParent=this.point.element.offsetParent,this.notStaticParent=null!==this.notStaticParent?this.notStaticParent:{x:0,y:0,scrollTop:0,scrollLeft:0},this.notStaticParent=this.isStatic(this.notStaticParent)?window:this.notStaticParent,this.update()};function x(e){return new b(e,h.getClientRectWithMargins)}function D(e){return new b(e,h.getClientRect)}P.prototype.isElementScrollable=function(e){var t=window.getComputedStyle(e);return"static"===t.position&&("scroll"===t["overflow-x"]||"scroll"===t["overflow-y"]||"auto"===t["overflow-x"]||"auto"===t["overflow-y"])},P.prototype.isStatic=function(e){return!!e.style&&"static"===window.getComputedStyle(e).position},P.prototype.update=function(){var e=this.notStaticParent===window?{x:window.scrollX,y:window.scrollY}:{x:this.notStaticParent.scrollLeft,y:this.notStaticParent.scrollTop},t=h.geometry(this.anchor.location,"+",this.point.delta,"-",this.notStaticParent,"+",e),n=this.point.element;o.changeDom((function(){n.style.left=t.x+"px",n.style.top=t.y+"px"}))},P.prototype.checkInCasesInt=function(e){var t=this.events;e.forEach((function(e){t.attachEvent(e[0],e[1],(function(e){this.update()}))}))},P.prototype.checkInCases=function(){return this.containers?(this.checkInCasesInt(Array.from(arguments)),this.update(),this):this},P.prototype.dispose=function(){this.events&&(this.events.dispose(),this.events=null,this.dropDownStartPos=null,this.containers=null,this.resizeObserver&&this.resizeObserver.disconnect())};var T,S={Popup:0,Modal:1},B=window.console,N=1,R=2,O=new WeakMap;function M(e){return e.compareDocumentPosition(document.body)&window.Node.DOCUMENT_POSITION_DISCONNECTED}function k(e){var t=(e=o.ensureElement(e)).querySelector("div.dxbs-dm.dropdown-menu");return t||(t=e.querySelector("div.dxgvCSD.dxbs-grid-vsd")),t}function z(e,t,n,i){return new Promise((function(n,r){var s=k(e);return(s=o.ensureElement(s))?(0===i&&(s.style.minWidth=e.offsetWidth+"px"),2===i&&(s.style.width=e.offsetWidth+"px"),I(e,s,t),a.scrollToFocusedItem(s),n()):n()}))}function I(e,t,n,i){O.has(e)&&(O.get(e).disconnect(),O.delete(e));var r=t.offsetParent;if(!r)return o.changeDom((function(){t.style.visibility=""})),void 0;var s,c=r.getBoundingClientRect(),a=e.getBoundingClientRect(),l=a.top-c.top,u=c.bottom-a.bottom,d=window.getComputedStyle(t),h=t.offsetHeight+Math.max(parseFloat(d.marginTop),0)+Math.max(parseFloat(d.marginBottom),0)+8;switch(n){case R:s=!0,c.top+(l-h)<=0&&c.top+l+e.offsetHeight+h+window.pageYOffset<=Math.max(document.body.scrollHeight,window.innerHeight)&&(s=!1);break;case N:default:s=!1,c.bottom-(u-h)>=window.innerHeight&&c.top+l-h+window.pageYOffset>=0&&(s=!0)}var f=a.left+t.offsetWidth+8>=document.body.clientWidth;o.changeDom((function(){T&&T.dispose(),T=s?f?x(t).rightBottomCorner.anchorTo(D(e).rightTopCorner):x(t).leftBottomCorner.anchorTo(D(e).leftTopCorner):f?x(t).rightTopCorner.anchorTo(D(e).rightBottomCorner):x(t).leftTopCorner.anchorTo(D(e).leftBottomCorner),t.style.visibility=""}))}var L=[{value:0,text:""},{value:1,text:"above"},{value:2,text:"below"},{value:4,text:"top-sides"},{value:8,text:"bottom-sides"},{value:16,text:"outside-left"},{value:32,text:"outside-right"},{value:64,text:"left-sides"},{value:128,text:"right-sides"}];function H(e,t,n){for(var o=e.target;o;){if(o===t)return!1;o=o.parentElement}n&&n()}function W(e){return"hidden"!==e.style.visibility||e.classList.contains("visually-hidden")}var F="a[href], input:not([disabled]), button:not([disabled]), [tabindex]:not([tabindex='-1'])";function U(e,t){if(o.getParentByClassName(e.srcElement,"modal-header"))return!0;var n=o.getParentByClassName(e.srcElement,"column-chooser-elements-container"),i=o.getParentByClassName(n,"modal-body");if(!n||!i)return!1;if(i.clientHeight>=n.clientHeight)return!0;var r=e.touches[0].pageY-t.touches[0].pageY,s=function(e){return e.getBoundingClientRect().top},c=n.querySelector(".column-chooser-element-container");if(c&&s(c)===s(i)&&r>=0&&e.cancelable)return!0;var a=function(e){return Math.round(e.getBoundingClientRect().bottom)},l=n.querySelector(".column-chooser-element-container:last-child");return!!(l&&a(l)===a(i)&&r<=0&&e.cancelable)||void 0}function q(e,t,n){if(n===S.Modal){var i=e.getBoundingClientRect(),r=t.getBoundingClientRect(),s=o.getParentByClassName(e,"dxbs-gridview"),c=s&&s.querySelector("thead");if(c){var a=c.getBoundingClientRect(),l=(a||i).bottom;return l>.5*t.clientHeight?l-i.top<.5*t.clientHeight?l-r.top-.5*t.clientHeight:i.top-r.top-2:void 0}}}function A(e){var t=e.getElementsByClassName("modal-body")[0];t.style.width=t.offsetWidth+"px",t.style.height=t.offsetHeight+"px"}var _={init:function(e,t,n,c){if(e=o.ensureElement(e),t=o.ensureElement(t),n=o.ensureElement(n),e){if(r.disposeEvents(e),n){var a=function(i){return H(i,e,(function(){o.elementIsInDOM(e)||r.disposeEvents(e);var i=document.activeElement===t,s=n&&W(n);(i||s)&&c.invokeMethodAsync("OnDropDownLostFocus",t.value).catch((function(e){return B.error(e)}))}))};i.attachEventToElement(document,s.TouchUIHelper.touchMouseDownEventName,a),r.registerDisposableEvents(e,(function(){i.detachEventFromElement(document,s.TouchUIHelper.touchMouseDownEventName,a)}))}return Promise.resolve("ok")}},dispose:function(e){return(e=o.ensureElement(e))&&(clearTimeout(e._closeTimerId),r.disposeEvents(e)),T&&T.dispose(),Promise.resolve("ok")},showAdaptiveDropdown:function(e,t,n,i,r,a){e=o.ensureElement(e);var d=o.getParentByClassName(e,n);if(d){var h=document.documentElement,w=document.documentElement.style.overflow,y=document.body.style.overflow,b=document.body.scroll,C=t.dialogType,E=t.alignment,j=C===S.Modal;C===S.Popup?0===E?I(d,e,t.dropDownDirection):l.show(e,d,function(e){var t="";return L.forEach((function(n){0!=(n.value&e)&&(t&&(t+=" "),t+=n.text)})),t}(E)):(e.style.paddingRight=l.getElementPaddingRight(e)+l.getScrollbarWidth()+"px",document.body.style.paddingRight=parseFloat(l.getElementPaddingRight(document.body))+l.getScrollbarWidth(),document.body.classList.add("modal-open"));var P=!1;return h.addEventListener(s.TouchUIHelper.touchMouseDownEventName,x),e.addEventListener("keydown",(function(t){t.keyCode===c.Key.Esc&&T(e)})),e.addEventListener("focusout",(function(t){var n,o,i;P||(null===t.relatedTarget||e.contains(t.relatedTarget)?null===t.relatedTarget&&h.addEventListener("focusin",D):void((n=e.compareDocumentPosition(t.relatedTarget))&window.Node.DOCUMENT_POSITION_PRECEDING?(i=e.querySelectorAll(F),void((o=i[i.length-1])&&o.focus())):n&window.Node.DOCUMENT_POSITION_FOLLOWING&&R()))})),function(e,t,n,o){e&&t.addEventListener("touchmove",(function(e){e.srcElement===t&&e.preventDefault()}));if(o===S.Modal){var i=null;n.addEventListener("touchstart",(function(e){i=e})),n.addEventListener("touchmove",(function(e){U(e,i)&&e.preventDefault(),i=e}))}}(j,e,d,C),function(e){if(m.has(e))return m.get(e);var t=new Promise((function(t){0===g.size&&v.observe(f,p);g.set(e,(function(){e=null,t()}))}));return m.set(e,t),t}(e).then((function(){C===S.Modal&&(document.body.classList.remove("modal-open"),document.body.style.paddingRight=parseFloat(l.getElementPaddingRight(document.body))-l.getScrollbarWidth()),N(),e=null})),k(C,!0),C===S.Popup?o.changeDom((function(){e&&(t.showFocus||(o.addClassNameToElement(e,"dxbs-focus-hidden"),u.initFocusHidingEvents(e)),R(),function(e,t){var n=o.getParentByClassName(e,t);if(e&&n){var i=o.getParentByClassName(e,"dx-menu-bar");i&&i.classList.contains("vertical")||(o.subscribeElementContentSize(n,r),r())}function r(){var t=e.getBoundingClientRect(),i=n.getBoundingClientRect(),r=parseFloat(e.style["margin-left"]);if(r&&(t.x=t.x-r),t.x+t.width>i.x+i.width&&i.width>t.width){var s=o.getParentByClassName(e.parentNode,"dropdown-menu");if(s){var c=s.getBoundingClientRect(),a=parseFloat(window.getComputedStyle(s,null).getPropertyValue("border-right-width"));e.style["margin-left"]="-"+(t.x+t.width-c.x-a)+"px"}else e.style["margin-left"]="-"+(t.x+t.width-i.x-i.width)+"px"}else r&&(e.style["margin-left"]="")}}(e,a))})):(window.addEventListener("resize",O),O()),Promise.resolve()}function x(t){(!e.contains(t.srcElement)||C===S.Modal&&e===t.srcElement)&&T(e)}function D(t){h.removeEventListener("focusin",D),null===t.relatedTarget&&t.target&&e&&e.contains(t.target)&&t.target.focus()}function T(e){if(!P){if(P=!0,M(e))return;e._closeTimerId=setTimeout((function(){N(),r.invokeMethodAsync("CloseDialog").catch((function(t){M(e)||B.error(t)}))}),200)}}function N(){h.removeEventListener(s.TouchUIHelper.touchMouseDownEventName,x),window.removeEventListener("resize",O),k(C,!1),e=null}function R(){var t=e.querySelector(F);t&&t.focus()}function O(){if(o.getParentByClassName(e,"modal-dialog-owner")){var t=e.firstElementChild.classList,n=q(d,h,C),i=h.clientHeight>h.clientWidth?"topVertical":"topHorizontal";o.RequestAnimationFrame((function(){t.contains("topVertical")||t.contains("topHorizontal")?t.remove("transition"):t.add("transition"),n&&(h.scrollTop=n),t.remove("topVertical","topHorizontal"),t.add(i)}))}}function k(e,t){var n,o,i;e===S.Modal&&(t?(n="hidden",o="hidden",i="no"):(n=w,o=y,i=b),document.documentElement.style.overflow=n,document.body.style.overflow=o,document.body.scroll=i)}},updateGridDropDown:z,setInlineModalSize:A};n.DialogType=S,n.default=_,n.getDropDownElement=k,n.isDropDownVisible=W,n.onOutsideClick=H,n.scrollDropDown=q,n.setInlineModalSize=A,n.setPositionOfDropDown=I,n.shouldPreventTouchMove=U,n.updateGridDropDown=z}),["cjs-chunk-c5286524.js","cjs-chunk-0da7e9be.js","cjs-dom-utils-393f2f58.js","cjs-chunk-ad620f3e.js","cjs-chunk-843046a9.js","cjs-chunk-96905969.js","cjs-chunk-e9e6b6d6.js","cjs-grid-d812d2f2.js","cjs-grid-column-resize-b7516f73.js","cjs-dx-style-helper-616b781d.js","cjs-popup-utils-a5804500.js","cjs-focus-utils-0ac5f035.js","cjs-chunk-b8f9edc9.js","cjs-dragAndDropUnit-7f87e559.js"]);