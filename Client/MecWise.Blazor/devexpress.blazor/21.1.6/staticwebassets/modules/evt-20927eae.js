import{a as e,b as t,d as r}from"./dom-a06f5987.js";import{t as o}from"./touch-2ca1b361.js";var n=function(){function n(){}return n.preventEvent=function(e){e.preventDefault?e.preventDefault():e.returnValue=!1},n.getEventSource=function(t){return e.isDefined(t)?t.srcElement?t.srcElement:t.target:null},n.getEventSourceByPosition=function(t){return e.isDefined(t)?document.elementFromPoint&&void 0!==n.getEventX(t)&&void 0!==n.getEventY(t)?document.elementFromPoint(n.getEventX(t),n.getEventY(t)):t.srcElement?t.srcElement:t.target:null},n.getMouseWheelEventName=function(){return t.Browser.Safari?"mousewheel":t.Browser.NetscapeFamily&&t.Browser.MajorVersion<17?"DOMMouseScroll":"wheel"},n.isLeftButtonPressed=function(r){return!!o.TouchUtils.isTouchEvent(r)||!!(r=t.Browser.IE&&e.isDefined(event)?event:r)&&(t.Browser.IE&&t.Browser.Version<11?!!t.Browser.MSTouchUI||r.button%2==1:t.Browser.WebKitFamily?"pointermove"===r.type?1===r.buttons:1===r.which:t.Browser.NetscapeFamily||t.Browser.Edge||t.Browser.IE&&t.Browser.Version>=11?r.type===o.TouchUtils.touchMouseMoveEventName?1===r.buttons:1===r.which:!t.Browser.Opera||0===r.button)},n.preventEventAndBubble=function(e){n.preventEvent(e),e.stopPropagation&&e.stopPropagation(),e.cancelBubble=!0},n.clientEventRequiresDocScrollCorrection=function(){return t.Browser.AndroidDefaultBrowser||t.Browser.AndroidChromeBrowser||!(t.Browser.Safari&&t.Browser.Version<3||t.Browser.MacOSMobilePlatform&&t.Browser.Version<5.1)},n.getEventX=function(e){return o.TouchUtils.isTouchEvent(e)?o.TouchUtils.getEventX(e):e.clientX+(n.clientEventRequiresDocScrollCorrection()?r.DomUtils.getDocumentScrollLeft():0)},n.getEventY=function(e){return o.TouchUtils.isTouchEvent(e)?o.TouchUtils.getEventY(e):e.clientY+(n.clientEventRequiresDocScrollCorrection()?r.DomUtils.getDocumentScrollTop():0)},n.cancelBubble=function(e){e.cancelBubble=!0},n.getWheelDelta=function(e){var r;return r=t.Browser.NetscapeFamily&&t.Browser.MajorVersion<17?-e.detail:t.Browser.Safari?e.wheelDelta:-e.deltaY,t.Browser.Opera&&t.Browser.Version<9&&(r=-r),r},n}();Object.defineProperty({EvtUtils:n},"__esModule",{value:!0});export{n as E};
