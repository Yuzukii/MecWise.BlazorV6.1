import"./esm-chunk-d81494b9.js";import"./esm-chunk-a2731447.js";import"./esm-dom-utils-0e4190ff.js";const n=[],i={width:0,height:0};window.addEventListener("resize",(function(e){const o=window.innerWidth,t=window.innerHeight;i.height===t&&i.width===o||(i.height=t,i.width=o,n.forEach((function(n){n(i)})))}),{passive:!0});const e=window.console;function o(i){return!function(i){if(n.indexOf(i)>-1)throw new Error("already subscribed");return n.push(i),function(){const e=n.indexOf(i);if(-1===e)throw new Error("already un-subscribed");n.splice(e,1)}}((function(n){o(i,n.width)})),o(i,window.innerWidth),Promise.resolve("ok");function o(n,i){n.invokeMethodAsync("OnWindowResize",i).catch(n=>e.error(n))}}function t(){return Promise.resolve("ok")}const r={init:o,dispose:t};export default r;export{t as dispose,o as init};