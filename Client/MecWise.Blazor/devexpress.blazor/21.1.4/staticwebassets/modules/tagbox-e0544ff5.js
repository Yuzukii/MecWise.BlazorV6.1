import{k as e}from"./key-643beca5.js";import{T as t}from"./touch-2ca1b361.js";import{d as o,r as n}from"./disposable-35c3c450.js";import{getDropDownElement as s,onOutsideClick as r,isDropDownVisible as i}from"./dropdowns-d4e1f7ff.js";import{A as a,e as c,g as d,a as u}from"./dom-utils-d8c2ed7a.js";import{scrollToFocusedItem as l,getParametersForVirtualScrollingRequest as m}from"./grid-adc97dfb.js";import{n as f,i as v,a as p}from"./keyboard-444ee1a1.js";import"./dom-a06f5987.js";import"./popup-utils-5ca88cd0.js";import"./focus-utils-1ab9273a.js";import"./dragAndDropUnit-238107dc.js";import"./evt-20927eae.js";import"./column-resize-9da63f0f.js";import"./dx-style-helper-bbeeef94.js";function y(e){const t=s(e);l(t)}function g(e,t,o){a(e,"value",t),E(e),o&&function(e){e&&e.select()}(e)}function E(e){if(!e.previousSibling)return;const t=e.previousSibling;t.innerText=""===e.value&&e.placeholder?e.placeholder:e.value}function k(e){e=c(e),document.activeElement===e&&C(e)}function L(e,t){const o=e.target;if(!o)return;b(o,!0),function(e){if(e){const t=d(e,"form-control");t.dataset.bluredClass&&(t.className=t.dataset.bluredClass)}}(o);const n=setTimeout((function(){if(delete o.dataset.timerId,document.activeElement!==o)try{T(t,o,!1)}catch(e){}}),200);o.dataset.timerId=String(n)}function b(e,t){if(e.dataset.timerId){const t=parseInt(e.dataset.timerId);clearTimeout(t),delete e.dataset.timerId}t||setTimeout((function(){b(e,!0)}),100)}function C(e){if(e){const t=d(e,"form-control");t.dataset.focusedClass&&(t.className=t.dataset.focusedClass)}}function h(t,o,n,s,r){f(t)&&(t.stopPropagation(),t.preventDefault());(function(t){const o=t.keyCode===e.KeyCode.Esc||t.keyCode===e.KeyCode.Enter;return v(t)||p(t)||o||function(t){const o=0===t.target.value.length;return t.keyCode===e.KeyCode.Backspace&&o}(t)})(t)&&!t.repeat&&function(e,t,o,n){if(!e.target)return;const s=e.target.value;if(o&&n){const n=o.querySelector(".dxgvCSD");n&&(o=n);const r=m(o);t.invokeMethodAsync("TagBoxWithVirtualScrollingOnKeyDown",s,e.keyCode,e.altKey,e.ctrlKey,r.requestScrollState.itemHeight,r.requestScrollState.scrollTop,r.requestScrollState.scrollHeight)}else t.invokeMethodAsync("TagBoxOnKeyDown",s,e.keyCode,e.altKey,e.ctrlKey)}(t,o,n,r)}function T(e,t,o){if(t){if(t.dataset.lastLostFocusTime){if((new Date).getTime()-parseInt(t.dataset.lastLostFocusTime)<300&&!o)return}t.dataset.lastLostFocusTime=String((new Date).getTime()),e.invokeMethodAsync("OnTagBoxLostFocus",t.value),document.activeElement!==t&&(t.value="")}}function j(a,d,l){const m=c(a);if(!m)return Promise.reject("failed");o(m);const v=c(d.inputElement),p=s(m),y=e=>h(e,l,p,0,d.virtualScrollingEnabled),g=t=>function(t,o){f(t)&&(t.stopPropagation(),t.preventDefault(),t.keyCode!==e.KeyCode.Up&&t.keyCode!==e.KeyCode.Down||o.invokeMethodAsync("OnTagBoxArrowKeyUp",t.keyCode===e.KeyCode.Up))}(t,l),k=e=>function(e){C(e.target)}(e),j=e=>function(e,t,o){o&&t.invokeMethodAsync("OnTagBoxProcessFiltering",e.target.value)}(e,l,d.filteringEnabled),S=e=>function(e,t,o){t.isDisabled||o.invokeMethodAsync("SetDropDownVisible"),t.enabled&&e.focus()}(v,d,l),w=e=>L(e,l),D=e=>E(e.target),I=e=>b(v,!1),K=e=>r(e,m,(()=>{u(m)||o(m);const e=document.activeElement===v,t=v.dataset.timerId>0,n=p&&i(p);b(v,!1),(e||t||n)&&T(l,v,!0)}));return v.addEventListener("keydown",y),v.addEventListener("keyup",g),v.addEventListener("focus",k),v.addEventListener("blur",w),v.addEventListener("input",D),v.addEventListener("input",j),m.addEventListener("click",S),m.addEventListener("mousedown",I),document.addEventListener(t.touchMouseDownEventName,K),n(m,(()=>{v.removeEventListener("keydown",y),v.removeEventListener("keyup",g),v.removeEventListener("focus",k),v.removeEventListener("blur",w),v.removeEventListener("input",D),v.removeEventListener("input",j),m.removeEventListener("click",S),m.removeEventListener("mousedown",I),document.removeEventListener(t.touchMouseDownEventName,K)})),E(v),Promise.resolve("ok")}function S(e){return(e=c(e))&&o(e),Promise.resolve("ok")}const w={init:j,dispose:S,prepareInputIfFocused:k,scrollToFocusedItem:y,fitInputWidth:E,forceInputValue:g};export default w;export{S as dispose,E as fitInputWidth,g as forceInputValue,j as init,k as prepareInputIfFocused,y as scrollToFocusedItem};
