import{D as t}from"./dom-a06f5987.js";import{c as e,p as i,v as o,w as n,j as r,m as s}from"./dom-utils-d8c2ed7a.js";const l="\\s*matrix\\(\\s*"+[0,0,0,0,0,0].map((function(){return"(\\-?\\d+\\.?\\d*)"})).join(",\\s*")+"\\)\\s*";function a(t){let e=0;if(null!=t&&""!==t)try{const i=t.indexOf("px");i>-1&&(e=parseFloat(t.substr(0,i)))}catch(t){}return Math.ceil(e)}function h(t){const e=new RegExp(l).exec(t.transform);return e?{left:parseInt(e[5]),top:parseInt(e[6])}:{left:0,top:0}}function f(t,e,i){t.transform="matrix(1, 0, 0, 1, "+e+", "+i+")"}function d(t,e,i){const o=t.getBoundingClientRect(),n={left:e(o.left),top:e(o.top),right:i(o.right),bottom:i(o.bottom)};return n.width=n.right-n.left,n.height=n.bottom-n.top,n}function m(t){return d(t,Math.floor,Math.ceil)}function c(t){return d(t,Math.ceil,Math.floor)}class g{constructor(t,e){this.key=t,this.info=e}checkMargin(){return!0}allowScroll(){return"height"===this.info.size}canApplyToElement(t){return t.className.indexOf("dxbs-align-"+this.key)>-1}getRange(t){const e=this.getTargetBox(t)[this.info.to],i=e+this.info.sizeM*(t.elementBox[this.info.size]+(this.checkMargin()?t.margin:0));return{from:Math.min(e,i),to:Math.max(e,i),windowOverflow:0}}getTargetBox(t){return null}validate(t,e){const i=e[this.info.size];return t.windowOverflow=Math.abs(Math.min(0,t.from-i.from)+Math.min(0,i.to-t.to)),t.validTo=Math.min(t.to,i.to),t.validFrom=Math.max(t.from,i.from),0===t.windowOverflow}applyRange(t,e){e.appliedModifierKeys[this.info.size]=this.key;const i="width"===this.info.size?"left":"top",o=e.styles;let n=t.from;this.allowScroll()&&t.windowOverflow>0&&(e.limitBox.scroll.width||(e.limitBox.scroll.width=!0,e.limitBox.width.to-=s()),e.isScrollable&&(o["max-height"]=e.height-t.windowOverflow+"px",e.width+=s(),e.elementBox.width+=s(),n=t.validFrom)),o.width=e.width+"px",this.checkMargin()&&(n+=Math.max(0,this.info.sizeM)*e.margin),e.elementBox[i]+=n,f(o,e.elementBox.left,e.elementBox.top)}dockElementToTarget(t){const e=this.getRange(t);this.dockElementToTargetInternal(e,t)||this.applyRange(e,t)}dockElementToTargetInternal(t,e){}}class u extends g{constructor(t,e,i){super(t,e,i),this.oppositePointName=i||null}getTargetBox(t){return t.targetBox.outer}getOppositePoint(){return this._oppositePoint||(this._oppositePoint=w.filter(function(t){return t.key===this.oppositePointName}.bind(this))[0])}dockElementToTargetInternal(t,e){if(this.validate(t,e.limitBox))this.applyRange(t,e);else{const i=this.getOppositePoint(),o=i.getRange(e);if(!(i.validate(o,e.limitBox)||o.windowOverflow<t.windowOverflow))return!1;i.applyRange(o,e)}return!0}}class p extends g{checkMargin(){return!1}getTargetBox(t){return t.targetBox.inner}dockElementToTargetInternal(t,e){return this.validate(t,e.limitBox),!1}validate(t,e){const i=Math.min(t.from,Math.max(0,t.to-e[this.info.size].to));return i>0&&(t.from-=i,t.to-=i),super.validate(t,e)}}const w=[new u("above",{to:"top",from:"bottom",size:"height",sizeM:-1},"below"),new u("below",{to:"bottom",from:"top",size:"height",sizeM:1},"above"),new p("top-sides",{to:"top",from:"top",size:"height",sizeM:1}),new p("bottom-sides",{to:"bottom",from:"bottom",size:"height",sizeM:-1}),new u("outside-left",{to:"left",from:"right",size:"width",sizeM:-1},"outside-right"),new u("outside-right",{to:"right",from:"left",size:"width",sizeM:1},"outside-left"),new p("left-sides",{to:"left",from:"left",size:"width",sizeM:1}),new p("right-sides",{to:"right",from:"right",size:"width",sizeM:-1})];function x(o,n,s,l){return e((function(){const e=function(e,i,o){const n=r(),s=m(e),l=c(i),f=e.ownerDocument.documentElement,d={isScrollable:t.hasClassName(e,"dxbs-scrollable"),specifiedOffsetModifiers:w.filter((function(t){return t.canApplyToElement(e)})),margin:a(n.marginTop),width:o?Math.max(o.width,Math.ceil(e.offsetWidth)):Math.ceil(e.offsetWidth),height:Math.ceil(e.offsetHeight),appliedModifierKeys:{height:null,width:null}},g=h(n),u=e.classList.contains("dxbs-visually-hidden")?l.left:s.left;var p,x,M,b;d.elementBox={left:p=g.left-u,top:x=g.top-s.top,right:p+(M=s.width),bottom:x+(b=s.height),width:M,height:b},d.targetBox={outer:m(i),inner:c(i)},d.limitBox={scroll:{width:f.clientWidth<window.innerWidth,height:f.clientHeight<window.innerHeight},width:{from:0,to:f.clientWidth},height:{from:0,to:f.clientHeight}},d.styles={};const z=(e.getAttribute("data-popup-align")||o&&o.align).split(/\s+/);return d.offsetModifiers=w.filter((function(t){return z.some((function(e){return t.key===e}))})),d}(o,n,s);for(let t=0;t<e.offsetModifiers.length;t++)e.offsetModifiers[t].dockElementToTarget(e);l(e),i(o,e.styles)}))}function M(t,e,i){null!==e&&(x(t,e,{align:i},(()=>{})),o(t,"show",!0),n(t))}function b(t){return parseFloat(window.getComputedStyle(t,null).getPropertyValue("padding-right"))}function z(){return window.innerWidth-document.body.getBoundingClientRect().width}export{z as a,b as g,h as p,M as s,f as t};
