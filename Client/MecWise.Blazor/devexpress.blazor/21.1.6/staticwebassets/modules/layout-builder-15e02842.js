import{K as e,j as t,I as r,L as n}from"./dom-utils-69ea39c0.js";import{D as o}from"./dom-a06f5987.js";const s={};var l,i;s.getIsParent=function(e,t){if(!e||!t)return!1;for(;t;){if(t===e)return!0;if("BODY"===t.tagName)return!1;t=t.parentNode}return!1},s.getElementOffsetWidth=function(e){return"svg"===e.tagName?e.getBoundingClientRect().width:e.offsetWidth},s.getChildElementNodes=function(e){return e?o.getChildNodes(e,(function(e){return 1===e.nodeType})):null},s.getChildElementNodesByPredicate=function(e,t){return e?t?o.getChildNodes(e,(function(e){return 1===e.nodeType&&t(e)})):s.getChildElementNodes(e):null},s.getChildByClassName=function(t,r,n){if(null!=t){const s=o.getChildNodesByClassName(t,r);return e(s,n)}return null},s.elementMatchesSelector=(l=Element.prototype,i=l.matches||l.matchesSelector||l.webkitMatchesSelector||l.mozMatchesSelector||l.msMatchesSelector||l.oMatchesSelector,function(e,t){return!!e&&!!t&&i.call(e,t)});class a{constructor(e){this.container=null,this.owner=e}querySelectorAll(e){var t;return this.querySelectorAllInternal(e||"#"+(null===(t=this.getContainer())||void 0===t?void 0:t.id))}toggleClassName(e,t,r){return this.owner?this.owner.toggleClassName(e,t,r):this.toggleClassNameInternal(e,t,r)}toggleClassNameInternal(e,t,r){}querySelectorAllInternal(e){return this.owner?this.owner.querySelectorAll(e):this.getNodes(e)}getNodes(e){const t=this.container;let r=t.querySelectorAll(e);return r.length||(e="#"+t.id+e,t.parentNode&&(r=t.parentNode.querySelectorAll(e))),r}getContainer(){return this.owner?this.owner.getContainer():this.container}getBoxSize(e){return Math.ceil(s.getElementOffsetWidth(e))+this.getBoxOuterOffset(e)}getBoxInnerOffset(e){return t(e)}getBoxOuterOffset(e){return r(e)}getBoxOffset(e){return this.getBoxOuterOffset(e)+this.getBoxInnerOffset(e)}getNodeWidth(e,t=!1){const r=e;return r?Math.ceil(r.offsetWidth+(t?0:this.getBoxOuterOffset(r)))+n(r):0}dispose(){}setActive(e){}createLayoutEntity(e,...t){return null!==this.owner?this.owner.createLayoutEntity(e,...t):this.createLayoutEntityCore(e,...t)}createLayoutEntityCore(e,...t){return new(this.resolveLayoutEntityType(e))(...t)}resolveLayoutEntityType(e){return e}}export{a as L,s as U};
