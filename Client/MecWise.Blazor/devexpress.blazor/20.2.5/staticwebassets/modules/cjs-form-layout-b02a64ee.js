DxBlazorInternal.define("cjs-form-layout-b02a64ee.js",(function(t,o,r){var e=t("./cjs-chunk-c5286524.js");t("./cjs-chunk-0da7e9be.js");var n=t("./cjs-dom-utils-393f2f58.js"),l={BootstrapMode:"Bootstrap4"};l.IsBootstrap3="Bootstrap3"===l.BootstrapMode,l.IsBootstrap4="Bootstrap4"===l.BootstrapMode,l.zIndexCategories={dropdown:1e3,sticky:1020,fixed:1030,modalBackdrop:1040,modal:1050,popover:1060,tooltip:1070};var d=l.IsBootstrap3?"panel":"card",i=l.IsBootstrap3?"control-label":"col-form-label",a=l.IsBootstrap3?"help-block":"form-text",s={};function f(t){!function(t,o,r){var e=s[t]||(s[t]=n.getCurrentStyle(document.body).getPropertyValue(t)||o);if(e){var l=n.getCurrentStyleSheet();l&&l.insertRule("@media (min-width: "+e+") {\n"+r+"\n}\n",l.cssRules.length)}}("--breakpoint-lg","992px",t)}var c=/\./g;function u(t){return t.replace(c,"\\$&")}function p(t){return document.getElementById(t)}function b(t,o){if(p(t)){var r=document.getElementById(o);r&&m(r,t,!0)}}function m(t,o,r){n.addClassNameToElement(t,"dxbs-fl-calc");(function(t,o,r,e){if(0===r)return;if(!o.itemCaptionWidth||r>o.itemCaptionWidth){o.itemCaptionWidth=r;var l=function(t,o){var r=p(t),e="#"+u(t)+".dxbs-fl.form-horizontal ";o!==r&&(e+="#"+u(o.id)+" ");var l=r;for(;l=n.getParentByClassName(l.parentNode,"dxbs-fl");)e="#"+u(l.id)+" "+e;return e}(t,o),d=e?" .row>div.dx-blazor-fl-tab-item > .form-group":"div:not(.dx-blazor-fl-tab-item) > .form-group",i=l+d+" > .dxbs-fl-cpt {\n width:"+r+"px;\n}\n";i+=l+d+" > .dxbs-fl-ctrl:not(img):not(.dxbs-fl-ctrl-nc) {\n width: calc(100% - "+r+"px);\n}\n",f(i+=l+(e?" .row > div > .":".row > div > .")+a+" {\n margin-left: "+r+"px;\n}\n")}})(o,t,function(t){var o=0;if(t.length>0){var r=t.find(x),n="0 px";r&&(n=window.getComputedStyle(r,null).getPropertyValue("height"));for(var l=0;l<t.length;l++){var d=t[l].offsetWidth;v(t[l])&&(t[l].style.height=n),d>o&&d<t[l].parentNode.offsetWidth&&(o=d)}}return o>0?o+(e.Browser.HardwareAcceleration?1:0):0}(r?function(t,o,r){if(r)return g(t,o.querySelectorAll("#"+o.id+" .row>div.dx-blazor-fl-tab-item>.form-group>.dxbs-fl-cpt."+i));return o.querySelectorAll("#"+o.id+" > .row > div:not(.dxbs-fl-g):not(.dxbs-fl-gd):not(.dxbs-fl-gt) > .form-group > ."+i+", #"+o.id+".dxbs-fl-gd > ."+d+" > .row > div:not(.dxbs-fl-g):not(.dxbs-fl-gd):not(.dxbs-fl-gt) > .form-group > ."+i)}(o,t,!0):function(t,o,r){if(r)return g(t,o.querySelectorAll("div:not(.dx-blazor-fl-tab-item)>.form-group>.dxbs-fl-cpt."+i));return o.querySelectorAll("#"+o.id+" > .row > div:not(.dxbs-fl-g):not(.dxbs-fl-gd):not(.dxbs-fl-gt) > .form-group > ."+i+", #"+o.id+".dxbs-fl-gd > ."+d+" > .row > div:not(.dxbs-fl-g):not(.dxbs-fl-gd):not(.dxbs-fl-gt) > .form-group > ."+i)}(o,t,!0)),r),n.removeClassNameFromElement(t,"dxbs-fl-calc")}function g(t,o){var r=p(t);return n.retrieveByPredicate(o,(function(t){return n.getParentByClassName(t,"dxbs-fl")===r}))}function x(t,o,r){return!v(t)}function v(t){return!!t.classList.contains("dxbs-fl-empty-caption")}function h(t){var o=p(t);o&&(m(o,t),o.removeAttribute("style"))}var y={init:h,createAdaptivityCssRulesForTab:b};r.createAdaptivityCssRulesForTab=b,r.default=y,r.init=h}),["cjs-chunk-c5286524.js","cjs-chunk-0da7e9be.js","cjs-dom-utils-393f2f58.js"]);
