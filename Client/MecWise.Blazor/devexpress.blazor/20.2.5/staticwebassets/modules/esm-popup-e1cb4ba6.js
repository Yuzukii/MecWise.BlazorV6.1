import"./esm-chunk-d81494b9.js";import"./esm-chunk-a2731447.js";import"./esm-dom-utils-0e4190ff.js";import{getElementPaddingRight as o,getScrollbarWidth as t}from"./esm-popup-utils-f634e322.js";function e(e,d,s){d!==s&&!function(e,d){let s=0;d?(e.style.paddingRight=o(e)+t()+"px",s=parseFloat(o(document.body))+t(),document.body.classList.add("modal-open")):(document.body.classList.remove("modal-open"),s=parseFloat(o(document.body))-t(),e.style.paddingRight=o(e)-t()+"px");document.body.style.paddingRight=s+"px"}(e,d),e.style.visibility=d?"visible":"hidden"}const d={renderModal:e};export default d;export{e as renderModal};