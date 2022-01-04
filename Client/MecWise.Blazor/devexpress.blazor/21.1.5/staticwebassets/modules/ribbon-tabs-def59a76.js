import{D as t}from"./dom-a06f5987.js";import{u as e,L as i,R as s,s as n,c as o,M as l,h as r,H as a}from"./dom-utils-417838ed.js";import{U as h,L as c}from"./layout-builder-49b0c35c.js";import{d as u,r as d}from"./disposable-35c3c450.js";class p extends c{constructor(t,e,i,s){super(t),this.builders=e||[],this.name=i,this.index=s?s.index+1:0,this.prevState=s,this.nextState=null,s&&(s.nextState=this)}for(t){return this.builders[this.builders.length]=this.createLayoutEntity(b,this.owner,this.name,t)}}class g extends c{constructor(t,e,i,s,n,o,l){super(t),this.onEnter=o,this.onLeave=l,this.prevBlock=null,this.nextBlock=null;const r=e;if(r.block&&(this.prevBlock=r.block,this.prevBlock&&(this.prevBlock.nextBlock=this)),r.block=this,this.isApplied=!1,this.node=e,this.className=s,this.blocksContainer=i,this.widthRanges=[],this.blocksContainerOffset=this.getBoxInnerOffset(i),this.minContainerWidth=0,this._isActive=n,h.getIsParent(i,e.parentNode))for(;e.parentNode!==i;)this.blocksContainerOffset+=this.getBoxInnerOffset(e=e.parentNode);this.minContainerWidth=this.node.offsetWidth,this.ownerGroup=null}canBePushed(){return!this.isActive()&&this.prevBlock&&this.prevBlock.isActive()}canBePopped(){return this.isActive()&&this.prevBlock}push(){this._isActive=!0,this.prevBlock&&(this.prevBlock._isActive=!1)}pop(){this._isActive=!1,this.prevBlock&&(this.prevBlock._isActive=!0)}isActive(){return this._isActive}getWidth(t=!1){return t?this.widthRanges[0].maxWidth:this.widthRanges[0].minWidth}applyLayout(){!this.isApplied&&this.isActive()&&(this.nextBlock&&this.nextBlock.applyLayoutInternal(!1),this.prevBlock&&this.prevBlock.applyLayoutInternal(!1),this.applyLayoutInternal(!0))}applyLayoutInternal(t){this.isApplied!==t&&(this.toggleClassName(this.node,this.className,t),this.toggleClassName(this.blocksContainer,this.className,t),t?this.onEnter():this.onLeave()),this.isApplied=t}dispose(){this.node.block=null,this.ownerGroup=null,this.isApplied&&(this.toggleClassName(this.node,this.className,!1),this.toggleClassName(this.blocksContainer,this.className,!1))}}class f extends c{constructor(t,e){super(t),this.blocks=[],this.isDefault=e}isActive(){return this.blocks.some((function(t){return t.isActive()}))}addBlock(t){t.ownerGroup=this,this.blocks.push(t)}applyLayout(){for(let t=0;t<this.blocks.length;t++)this.blocks[t].applyLayout()}getWidth(t=!1){let e=0,i=null;const s=this.getOrderedBlocks();for(let n=0;n<s.length;n++){const o=s[n];this.isDefault&&i!==o.blocksContainer&&(i=o.blocksContainer,e+=o.blocksContainerOffset),e+=o.getWidth(t)}return e}getOrderedBlocks(){return this.blocks}shrink(){const t=this.getOrderedBlocks();for(let e=t.length-1;e>=0;e--){const i=t[e];if(i.canBePushed())return i.push(),!0}return!1}grow(){const t=this.getOrderedBlocks();for(let e=0;e<t.length;e++){const i=t[e];if(i.canBePopped())return i.pop(),!0}return!1}}class b extends c{constructor(t,e,i){super(t),this.name=e,this.selectorOrFunc=i,this.onStateEnter=null,this.onStateLeave=null,this.prepareBlockFunc=null}getBlockWidth(t){return this.prepareBlockFunc?this.prepareBlockFunc(t):[]}findBlockElements(t){const e=h.getChildElementNodesByPredicate(t,(t=>h.elementMatchesSelector(t,this.selectorOrFunc)));return null!=e?e:[]}setPrepareFunc(t){return this.prepareBlockFunc=t.bind(this),this}buildBlock(t,e,i){const s=this.createLayoutEntity(g,this.owner,t,e,this.name,i,(()=>{var t;this.onStateEnter&&(null===(t=this.owner)||void 0===t||t.domChanges.push(this.onStateEnter.bind(this,s)))}),(()=>{var t;this.onStateLeave&&(null===(t=this.owner)||void 0===t||t.domChanges.push(this.onStateLeave.bind(this,s)))}));return this.prepareBlockFunc&&(s.widthRanges=this.prepareBlockFunc(s)),s}onEnterState(t){return this.onStateEnter=t,this}onLeaveState(t){return this.onStateLeave=t,this}setOverflow(){return this}fixTabsWidth(){return this}}class m extends b{fixTabsWidth(){return this.prepareBlockFunc=function(e){var i;const s=t.getNodesByClassName(null===(i=this.owner)||void 0===i?void 0:i.getContainer(),"nav-tabs")[0];if(!s)return[];const n=r(s),o=s.querySelectorAll("li"),l=[],a={minWidth:0,maxWidth:0};for(let t=0;t<o.length;t++)h=this.getNodeWidth(o[t]),l.push({width:h,isActive:!0});var h;return a.minWidth=a.maxWidth=l.filter((function(t){return t.isActive})).reduce((function(t,e){return t+e.width}),n),[a]},this}setOverflow(){return this.prepareBlockFunc=function(t){const e=h.getChildElementNodes(t.node);if(!e)return[];const i=r(t.node)+a(t.node);let s=0;for(let t=0;t<e.length;t++)s=Math.max(s,this.getNodeWidth(e[t]));return[{minWidth:i+s,maxWidth:i+s}]},this}}class v extends c{constructor(t,e,i){super(t),this.groupsContainer=e,this.groupsContainerOffset=this.getBoxInnerOffset(this.groupsContainer),this.selector=i,this.currentWidth=-1,this.maxWidth=-1,this.minWidth=-1,this.blockGroups=[],this.allBlocks=[],this.states=[]}createBlockGroups(){}defineState(t,e){const i=[];e(this.createLayoutEntity(p,this.owner,i,t));const s=this.getNodesOrContainerIfMatches(this.groupsContainer,this.selector),n=this.createBlockGroup();for(let t=0;t<s.length;t++){const e=s[t];for(let t=0;t<i.length;t++){const s=i[t],o=this.getNodesOrContainerIfMatches(e,s.selectorOrFunc);for(let t=0;t<o.length;t++){const i=s.buildBlock(o[t],e,n.isDefault);this.allBlocks.push(i),n.addBlock(i)}}}}getNodesOrContainerIfMatches(t,e){let i=Array.from(t.querySelectorAll(e));return 0===i.length&&h.elementMatchesSelector(t,e)&&(i=[t]),i}createBlockGroup(t=null){return this.blockGroups[this.blockGroups.length]=this.createLayoutEntity(f,this.owner,0===this.blockGroups.length,t)}getWidth(t=!1){let e=0;e+=this.groupsContainerOffset;let i=null,s=0;for(let n=0;n<this.allBlocks.length;n++){const o=this.allBlocks[n];o.isActive()&&(i!==o.blocksContainer&&this.groupsContainer!==o.blocksContainer&&(e+=s,s=0,i=o.blocksContainer,s+=o.blocksContainerOffset),s+=o.getWidth(t))}return e+=s,e}shrink(){for(let t=0;t<this.blockGroups.length;t++)if(this.blockGroups[t].shrink())return!0;return!1}grow(){for(let t=this.blockGroups.length-1;t>=0;t--)if(this.blockGroups[t].grow())return!0;return!1}applyLayout(){for(let t=0;t<this.blockGroups.length;t++)this.blockGroups[t].applyLayout()}adjust(t){if(this.currentWidth>t){for(;this.getWidth()>t&&this.shrink(););this.currentWidth=t,this.applyLayout()}else if(t>this.currentWidth){for(;t>this.getWidth()&&this.grow();)if(this.getWidth()>t){this.shrink();break}this.currentWidth=t,this.applyLayout()}}initialize(){this.currentWidth=this.getWidth(!1),this.minWidth=this.currentWidth,this.maxWidth=this.getWidth(!0)}dispose(){for(let t=0;t<this.allBlocks.length;t++)this.allBlocks[t].dispose();this.allBlocks=[],this.blockGroups=[]}}class k extends c{constructor(t,e){super(null),this.rebuildBreakpointsInfo=[],this.builderClass=e,this.control=t,this.container=t.getMainElement(),this.containerOffsets=this.calculateContainerOffsets(),this.blockGroupsArray=[],this.isReady=!1,this.classesToApply=[],this.domChanges=[],this.nextAdjustGroupWidth=null}resolveLayoutEntityType(t){return t===b?this.builderClass:t}toggleClassNameInternal(t,e,i){this.getBatchCssUpdateCache(t)[e]=i}getBatchCssUpdateCache(t){let e=t._layoutBuilderCache;return e||(e=t._layoutBuilderCache={},this.classesToApply.push(this.createBatchCssUpdateDelegate(t))),e}createBatchCssUpdateDelegate(e){return function(){const i=e._layoutBuilderCache;if(i){delete e._layoutBuilderCache;for(const s in i)Object.prototype.hasOwnProperty.call(i,s)&&t.toggleClassName(e,s,i[s])}}}createBlockGroups(t,e,i){this.rebuildBreakpointsInfo.push({selector:t,groupSelector:e,prepareFunc:i});const s=[],n=this.querySelectorAll(t);for(let t=0;t<n.length;t++){const o=this.createLayoutEntity(v,this,n[t],e);this.blockGroupsArray.push(o),i&&i(o),o.createBlockGroups(),s.push(o)}return this.nextAdjustGroupWidth=this.getGroupsWidth(),s}rebuild(){const t=this.rebuildBreakpointsInfo;this.dispose();for(let e=0;e<t.length;e++)this.createBlockGroups(t[e].selector,t[e].groupSelector,t[e].prepareFunc);s((()=>this.initialize()))}initialize(){for(let t=0;t<this.blockGroupsArray.length;t++)this.blockGroupsArray[t].initialize();this.isReady=!0,this.adjust()}adjust(t=null,e=null){if(this.isReady){const i=this.classesToApply,s=this.domChanges;let n,o=this.classesToApply=[],l=this.domChanges=[];null!==this.nextAdjustGroupWidth?(n=this.nextAdjustGroupWidth,this.nextAdjustGroupWidth=null):n=this.getGroupsWidth();for(let t=0;t<this.blockGroupsArray.length;t++)this.blockGroupsArray[t].adjust(n);t&&t(),o=i.concat(o),l=s.concat(l),this.queueUpdates((()=>{for(;o.length;){const t=o.shift();t&&t()}this.queueUpdates((()=>{for(;l.length;){const t=l.shift();t&&t()}e&&e()}))}))}}queueUpdates(t){s(t)}getGroupsWidth(){const t=this.getContainer();return t?t.offsetWidth-this.containerOffsets:0}dispose(){this.containerOffsets=this.calculateContainerOffsets();for(let t=0;t<this.blockGroupsArray.length;t++)this.blockGroupsArray[t].dispose();this.blockGroupsArray=[],this.isReady=!1,this.rebuildBreakpointsInfo=[]}calculateContainerOffsets(){return this.container?this.getBoxInnerOffset(this.container):0}}class y{constructor(t,e){this.mainElementContentWidthSubscription=null,this.contentPanelWidthSubscription=null,this.currentSize={width:null,height:null},this.contentSize={width:null,height:null},this.layoutBreakPoints=null,this.scrollContainerElement=null,this.canTrackScroll=!1,this.tabCount=0,this.dotNetReference=t,this.domChanges=[],this.mainElement=e.mainElement,this.tabCount=e.tabCount,this.layoutMode=e.layoutMode,this.activeTab=e.activeTab,this.id=e.id;const i=this.getTabsCell();null==i||i.addEventListener("scroll",(t=>this.onScroll(t))),null==i||i.addEventListener("click",(t=>this.activate(t)));const s=this.getPrevButton();null==s||s.addEventListener("click",(t=>this.scrollToPrev(t)));const n=this.getNextButton();null==n||n.addEventListener("click",(t=>this.scrollToNext(t)));d(this.mainElement,(()=>{null==i||i.removeEventListener("scroll",(t=>this.onScroll(t))),null==i||i.removeEventListener("click",(t=>this.activate(t))),null==s||s.removeEventListener("click",(t=>this.scrollToPrev(t))),null==n||n.removeEventListener("click",(t=>this.scrollToNext(t)))}))}dispose(){this.mainElementContentWidthSubscription&&e(this.mainElementContentWidthSubscription),this.contentPanelWidthSubscription&&e(this.contentPanelWidthSubscription)}activate(t){let e=t.target;for(;e;){if(i(e,"nav-item")){const t=h.getChildElementNodesByPredicate(e,(t=>h.elementMatchesSelector(t,".nav-link.active")));if(t&&t.length)return;if(null!=e.parentNode){const t=Array.from(e.parentNode.children).indexOf(e);this.scrollToIndex(t)}return}e=e.parentElement}}onScroll(t){this.updateOverflowState()}getMainElement(){return this.mainElement}initialize(){s((()=>{this.buildLayout()})),s((()=>{this.adjustLayout()})),this.mainElementContentWidthSubscription=n(this.getMainElement(),(t=>{this.currentSize.width===t.width&&this.currentSize.height===t.height||(this.currentSize={width:t.width,height:t.height},this.onMainElementResize())}));const t=document.querySelector(y.getSelector("data-dxtabs-content-id",this.id));t&&(this.contentPanelWidthSubscription=n(t,(t=>{this.contentSize.width===t.width&&this.contentSize.height===t.height||(this.contentSize={width:t.width,height:t.height},this.onContentElementResize())})))}onMainElementResize(){this.layoutBreakPoints&&this.layoutBreakPoints.adjust(),this.updateOverflowState()}onContentElementResize(){this.updateSelectedContent()}update(t){var e,i;this.tabCount!==t.tabCount?(null===(e=this.layoutBreakPoints)||void 0===e||e.rebuild(),this.tabCount=t.tabCount,this.activeTab=t.activeTab,this.updateSelectedContent()):null===(i=this.layoutBreakPoints)||void 0===i||i.adjust(null,(()=>{setTimeout((()=>{this.updateOverflowState()}),200)}))}buildLayout(){var t;this.layoutMode===C.Scroll&&(this.layoutBreakPoints=this.layoutBreakPoints||new k(this,m)),null===(t=this.layoutBreakPoints)||void 0===t||t.createBlockGroups(".dxbs-tabs-scrollable",".dxbs-tabs-scrollable",(t=>{t.defineState("no-overflow",(function(t){t.for(".dxbs-tabs-scrollable .nav").fixTabsWidth()})),t.defineState("has-overflow",(t=>{t.for(".dxbs-tabs-scrollable .nav").setOverflow().onEnterState((()=>this.onEnterOverflowState())).onLeaveState((()=>this.onLeaveOverflowState()))}))}))}adjustLayout(){var t;this.applyLayoutStateAppearance(),null===(t=this.layoutBreakPoints)||void 0===t||t.adjust()}applyLayoutStateAppearance(){var e;t.addClassName(this.getMainElement(),"dxbs-loaded"),null===(e=this.layoutBreakPoints)||void 0===e||e.initialize()}onEnterOverflowState(){this.canTrackScroll=!0,setTimeout((()=>{this.updateOverflowState()}))}onLeaveOverflowState(){this.canTrackScroll=!1}updateOverflowState(){if(!this.canTrackScroll)return;const e=this.findScrollContainer();if(!e)return;const i=this.getTabsCell(),s=i.scrollLeft,n=s>0,r=i.scrollWidth-s!==i.clientWidth,a=this.getNextButton(),h=this.getPrevButton();o((()=>{t.toggleClassName(e,"can-scroll-right",r),l(a,"disabled",!r),t.toggleClassName(e,"can-scroll-left",n),l(h,"disabled",!n)}))}findScrollContainer(){if(!this.scrollContainerElement){const t=this.getMainElement();this.scrollContainerElement=h.elementMatchesSelector(t,".dxbs-tabs-scrollable")?t:t.querySelector(".dxbs-tabs-scrollable")}return this.scrollContainerElement}getTabsCell(){return t.getNodesByClassName(this.getMainElement(),"nav-tabs")[0]}getPrevButton(){var t;return null===(t=this.findScrollContainer())||void 0===t?void 0:t.querySelector(".dxbs-tabs-scroll-btn.prev")}getNextButton(){var t;return null===(t=this.findScrollContainer())||void 0===t?void 0:t.querySelector(".dxbs-tabs-scroll-btn.next")}scrollToPrev(t){t.preventDefault();const e=this.findExtremeTabItemIndex(this.tabCount-1,0);this.scrollToIndex(e)}scrollToNext(t){t.preventDefault();const e=this.findExtremeTabItemIndex(0,this.tabCount-1);this.scrollToIndex(e)}isElementVisible(t){return!0}getTabElement(e,i){const s=this.getTabsCell(),n=t.getChildNodesByClassName(s,"nav-item");return Array.from(n)[e]}scrollToIndex(t){if(t>-1&&this.getTabsCell()){const e=(t,i)=>t&&!this.isElementVisible(t)?e(i(t),i):t;let i=this.getTabElement(t,!1);this.isElementVisible(i)||(i=this.getTabElement(t,!0));const s=e(i.nextElementSibling,(function(t){return t.nextElementSibling})),n=e(i.previousElementSibling,(function(t){return t.previousElementSibling}));this.scrollToTab(i,n,s)}}findExtremeTabItemIndex(t,e){const i=(e-t)/Math.abs(e-t),s=this.getContentFrameRect(this.getTabsCell()),n=e+i;for(;t!==n;t+=i){let e=this.getTabElement(t,!1);if(this.isElementVisible(e)||this.isElementVisible(e=this.getTabElement(t,!0))){const n=e.getBoundingClientRect();if(-1===i&&n.left<s.left||1===i&&n.right>s.right)return t}}return-1}getContentFrameRect(e){const i=t.getCurrentStyle(e),s=e.getBoundingClientRect();return{left:s.left+t.pxToInt(i.paddingLeft)+t.pxToInt(i.borderLeftWidth),right:s.right-t.pxToInt(i.paddingRight)-t.pxToInt(i.borderRightWidth),top:s.top+t.pxToInt(i.paddingTop)+t.pxToInt(i.borderTopWidth),bottom:s.bottom-t.pxToInt(i.paddingBottom)-t.pxToInt(i.borderBottomWidth)}}animateNonStyleProperty(t,e,i){const n=[];for(let t=99;t>=0;t--){const e=i(t);e!==n[n.length-1]&&n.push(e)}const o=setInterval((function(){var t;n.length?(t=n.shift(),s((function(){e(t)}))):clearInterval(o)}),1e3*t/n.length)}scrollToTab(t,e,i){const s=t.parentNode,n=t.getBoundingClientRect(),o=this.getContentFrameRect(s),l=e?e.getBoundingClientRect().right-e.getBoundingClientRect().width:n.left,r=i?i.getBoundingClientRect().left+i.getBoundingClientRect().width:n.right;let a=-Math.max(o.left-l,0)-Math.min(0,o.right-r);const h=s.scrollLeft;e||(a=-h),i||(a=Math.abs(o.right-h)),0!==Math.round(a)&&this.animateNonStyleProperty(.15,(function(t){s.scrollLeft=t}),(function(t){return Math.round(h+a+(1-a)*(1-Math.pow(1-t/100,4)))}))}changeSelection(t){this.activeTab=t.activeTab,this.updateSelectedContent()}updateSelectedContent(){const t=document.querySelector(y.getSelector("data-dxtabs-content-id",this.id));if(t){const e=document.querySelector(y.getSelector("data-dxtabs-content-id",this.activeTab)),i=Array.from(document.querySelectorAll(y.getSelector("data-dxtabs-content-id",this.id)+"> .dx-tabs-content")).filter((t=>t!==e));o((()=>{e&&(e.style.cssText="",e.setAttribute("data-dx-tab-loaded","")),o((()=>{const e=`position:absolute;visibility:hidden;left:-10000px;width:${t.clientWidth}px;height:${t.clientHeight}px`;i.forEach((t=>{t.style.cssText=e}))}))}))}}static getSelector(t,e){return`[${t}=${e}]`}}var C;!function(t){t[t.Default=0]="Default",t[t.Scroll=1]="Scroll"}(C||(C={}));const B=new Map;function S(t,e){const i=e.mainElement;let s=B.get(i);return s?s.update(e):(s=new y(t,e),s.initialize(),B.set(i,s)),Promise.resolve("ok")}function x(t,e){const i=B.get(e.mainElement);return i&&i.changeSelection(e),Promise.resolve("ok")}function E(t){if(t){u(t);const e=B.get(t);e&&(e.dispose(),B.delete(t))}return Promise.resolve("ok")}const W={init:S,dispose:E,changeSelection:x};export default W;export{x as changeSelection,E as dispose,S as init};