DxBlazorInternal.define("cjs-spinedit-59b013d9.js",(function(e,t,n){e("./cjs-chunk-c5286524.js"),e("./cjs-chunk-0da7e9be.js");var a=e("./cjs-dom-utils-393f2f58.js"),s=e("./cjs-chunk-ad620f3e.js"),o=e("./cjs-chunk-843046a9.js"),r=e("./cjs-chunk-e9e6b6d6.js");function c(e,t){e&&(e.dataset.previousValue=t)}function u(e,t,n){e=a.ensureElement(e);var c=a.getParentByClassName(e,"dxbs-spin-edit");if(e){o.disposeEvents(c);var u,d,i,l=(u=t.decimalSeparator,d=t.needExponentialView,i=e,function(e){var t=i.dataset.selectionStartBeforePaste?i.value.trim():i.value,n=/^-?(\d)*$/;u&&(t=t.replace(/[.|,]/g,u),n=d?/^-?(\d+|[,.]\d+|\d+[,.]\d+|\d+[,.]|[,.])?([eE]?[+-]?(\d)*)?$/:/^-?(\d+|[,.]\d+|\d+[,.]\d+|\d+[,.]|[,.])?$/);var a=i.selectionStart,s=i.selectionEnd;n.test(t)?(t!==i.value&&(i.value=t),i.dataset.previousValue=t):(i.dataset.selectionStartBeforePaste?(a=i.dataset.selectionStartBeforePaste,s=i.dataset.selectionEndBeforePaste):(a--,s+=i.dataset.previousValue.length-t.length),i.value=i.dataset.previousValue),i.dataset.selectionStartBeforePaste&&(i.removeAttribute("data-selection-start-before-paste"),i.removeAttribute("data-selection-end-before-paste")),i.selectionStart=a,i.selectionEnd=s}),v=function(e){return function(t){e.dataset.selectionStartBeforePaste=e.selectionStart,e.dataset.selectionEndBeforePaste=e.selectionEnd}}(e),f=function(t){if(function(e){return e.keyCode===r.Key.Up||e.keyCode===r.Key.Down}(t)){if(t.stopPropagation(),t.preventDefault(),t.repeat)return;n.invokeMethodAsync("OnKeyDown",t.keyCode===r.Key.Up,e.value)}},m=function(t){return function(e,t,n,a){if(document.activeElement!==n||!function(e){return!e.disabled&&!e.readOnly}(n)||"true"!==t.dataset.mouseOver)return;return e.preventDefault(),a.invokeMethodAsync("OnMouseWheel",e.deltaY<0,n.value),!1}(t,c,e,n)},E=function(e){return function(e){e.dataset.mouseOver=!0}(c)},h=function(e){return function(e){e.dataset.mouseOver=!1}(c)};return s.attachEventToElement(e,"input",l),s.attachEventToElement(e,"paste",v),s.attachEventToElement(e,"keydown",f),s.attachEventToElement(document,"wheel",m),s.attachEventToElement(c,"mouseenter",E),s.attachEventToElement(c,"mouseleave",h),o.registerDisposableEvents(c,(function(){s.detachEventFromElement(e,"input",l),s.detachEventFromElement(e,"keydown",f),s.detachEventFromElement(e,"paste",v),s.detachEventFromElement(document,"wheel",m),s.detachEventFromElement(c,"mouseenter",E),s.detachEventFromElement(c,"mouseleave",h)})),Promise.resolve("ok")}}function d(e,t){for(var n=document.activeElement;null!==n&&n!==e;)n=n.parentElement;null===n&&t.invokeMethodAsync("FocusLost")}function i(e){if(e=a.ensureElement(e))return o.disposeEvents(e),Promise.resolve("ok")}var l={init:u,dispose:i,setPreviousValue:c,checkFocusedState:d};n.checkFocusedState=d,n.default=l,n.dispose=i,n.init=u,n.setPreviousValue=c}),["cjs-chunk-c5286524.js","cjs-chunk-0da7e9be.js","cjs-dom-utils-393f2f58.js","cjs-chunk-ad620f3e.js","cjs-chunk-843046a9.js","cjs-chunk-e9e6b6d6.js"]);