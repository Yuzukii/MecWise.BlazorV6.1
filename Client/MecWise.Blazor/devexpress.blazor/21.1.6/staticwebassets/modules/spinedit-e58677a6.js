import{k as e}from"./key-643beca5.js";import{d as t,r as n}from"./disposable-35c3c450.js";import{e as a,g as s}from"./dom-utils-69ea39c0.js";import"./dom-a06f5987.js";function o(e,t){e&&(e.dataset.previousValue=t)}function r(o,r,i){o=a(o);const d=s(o,"dxbs-spin-edit");if(!o)return Promise.reject("failed");t(d);const l=e=>{var t,n,a;r.maskMode||(t=r.decimalSeparator,n=r.needExponentialView,a=o,e=>{let s=a.dataset.selectionStartBeforePaste?a.value.trim():a.value,o=/^-?(\d)*$/;t&&(s=s.replace(/[.|,]/g,t),o=n?/^-?(\d+|[,.]\d+|\d+[,.]\d+|\d+[,.]|[,.])?([eE]?[+-]?(\d)*)?$/:/^-?(\d+|[,.]\d+|\d+[,.]\d+|\d+[,.]|[,.])?$/);let r=a.selectionStart,i=a.selectionEnd;o.test(s)?(s!==a.value&&(a.value=s),a.dataset.previousValue=s):(a.dataset.selectionStartBeforePaste&&a.dataset.selectionEndBeforePaste?(r=parseInt(a.dataset.selectionStartBeforePaste),i=parseInt(a.dataset.selectionEndBeforePaste)):r&&i&&a.dataset.previousValue&&(r--,i+=a.dataset.previousValue.length-s.length),a.value=a.dataset.previousValue||""),a.dataset.selectionStartBeforePaste&&(a.removeAttribute("data-selection-start-before-paste"),a.removeAttribute("data-selection-end-before-paste")),a.selectionStart=r,a.selectionEnd=i})(e)},u=e=>{var t;r.maskMode||(t=o,e=>{t.dataset.selectionStartBeforePaste=String(t.selectionStart),t.dataset.selectionEndBeforePaste=String(t.selectionEnd)})(e)},c=t=>{if(function(t){return t.keyCode===e.KeyCode.Up||t.keyCode===e.KeyCode.Down}(t)){if(t.preventDefault(),t.repeat)return;const n=o.getAttribute("dx-mask-value");i.invokeMethodAsync("OnKeyDown",t.keyCode===e.KeyCode.Up,null==n?o.value:n)}},v=e=>function(e,t,n,a){return!(!function(e){return!e.disabled&&!e.readOnly}(n)||document.activeElement!==n)&&(e.preventDefault(),a.invokeMethodAsync("OnMouseWheel",e.deltaY<0),!1)}(e,0,o,i);return o.addEventListener("input",l),o.addEventListener("paste",u),o.addEventListener("keydown",c),d.addEventListener("wheel",v,{passive:!1}),n(d,(function(){o.removeEventListener("input",l),o.removeEventListener("keydown",c),o.removeEventListener("paste",u),d.removeEventListener("wheel",v)})),Promise.resolve("ok")}function i(e,t){d(e).then((e=>{e&&t.invokeMethodAsync("FocusLost")}))}function d(e){return new Promise(((t,n)=>{let a=document.activeElement;for(;null!==a&&a!==e;)a=a.parentElement;t(null!==a)}))}function l(e){return(e=a(e))&&t(e),Promise.resolve("ok")}const u={init:r,dispose:l,setPreviousValue:o,checkFocusedState:i,isFocusInsideSpinEdit:d};export default u;export{i as checkFocusedState,l as dispose,r as init,d as isFocusInsideSpinEdit,o as setPreviousValue};
