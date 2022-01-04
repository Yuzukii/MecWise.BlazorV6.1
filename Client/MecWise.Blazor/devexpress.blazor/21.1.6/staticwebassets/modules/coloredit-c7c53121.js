import{E as t}from"./evt-20927eae.js";import{T as e}from"./touch-2ca1b361.js";import{r,d as o}from"./disposable-35c3c450.js";import{e as i,J as n}from"./dom-utils-69ea39c0.js";import"./dom-a06f5987.js";class s{static hsbToHtml(t){const e=this.hsbToRgb(t);return"rgb("+e.red+", "+e.green+", "+e.blue+")"}static hsbToRgb(t){const e=Math.floor(t.hue/60),r=(s.maxHsb.saturation-t.saturation)*t.brightness/s.maxHsb.brightness,o=t.hue%60*(t.brightness-r)/60,i=r+o,n=t.brightness-o;let a={red:0,green:0,blue:0};switch(e){case 0:a={red:t.brightness,green:i,blue:r};break;case 1:a={red:n,green:t.brightness,blue:r};break;case 2:a={red:r,green:t.brightness,blue:i};break;case 3:a={red:r,green:n,blue:t.brightness};break;case 4:a={red:i,green:r,blue:t.brightness};break;case 5:a={red:t.brightness,green:r,blue:n}}return a.red=Math.round(2.55*a.red),a.green=Math.round(2.55*a.green),a.blue=Math.round(2.55*a.blue),a}}s.maxHsb={hue:359,saturation:100,brightness:100};class a{constructor(t,e){this.currentColor=s.maxHsb,this._dotnetReference=e;const r=t.querySelector(".dx-blazor-colorpicker-hue-scale-wrapper"),o=t.querySelector(".dx-blazor-colorpicker-pallete");if(!r||!o)throw new Error("ColorAreas elements not found");this._hueArea=this.createHueArea(r,this),this._colorArea=this.createSaturationBrightnessArea(o,this)}updateColor(t){this.currentColor=t,this._hueArea.updateColor(t),this._colorArea.updateColor(t)}dispose(){this._colorArea.dispose(),this._hueArea.dispose()}createHueArea(t,e){if(this._hueArea)return this._hueArea;return new c(t,(t=>{e.currentColor.hue=t.hue,e._colorArea.updateBackground(e.currentColor);const r=s.hsbToRgb(e.currentColor);e._dotnetReference.invokeMethodAsync("OnColorChanged",r.red,r.green,r.blue)}))}createSaturationBrightnessArea(t,e){if(this._colorArea)return this._colorArea;return new u(t,(t=>{e.currentColor.saturation=t.saturation,e.currentColor.brightness=t.brightness;const r=s.hsbToRgb(e.currentColor);e._colorArea.indicator.style.backgroundColor="rgb("+r.red+", "+r.green+", "+r.blue+")",e._dotnetReference.invokeMethodAsync("OnColorChanged",r.red,r.green,r.blue)}))}}class h{constructor(t,e){this.onColorChanged=e,this.container=t,this.indicator=t.querySelector(this.getIndicatorCssCelector()),this._containerSize={width:this.container.offsetWidth,height:this.container.offsetHeight},this._indicatorSize={width:this.indicator.offsetWidth,height:this.indicator.offsetHeight},this.initializeEvents(this)}getIndicatorHeight(){return this._indicatorSize.height}getIndicatorWidth(){return this._indicatorSize.width}getContainerHeight(){return this._containerSize.height}getContainerWidth(){return this._containerSize.width}initializeEvents(o){const i=t=>{e.isTouchEvent(t)&&t.preventDefault(),document.addEventListener(e.touchMouseUpEventName,n),document.addEventListener(e.touchMouseMoveEventName,s),o.moveIndicator(t)},n=t=>{document.removeEventListener(e.touchMouseUpEventName,n),document.removeEventListener(e.touchMouseMoveEventName,s)},s=r=>{e.isTouchEvent(r)&&r.preventDefault(),t.isLeftButtonPressed(r)&&o.moveIndicator(r)};this.container.addEventListener(e.touchMouseDownEventName,i),r(this.container,(()=>{o.container.removeEventListener(e.touchMouseDownEventName,i),document.removeEventListener(e.touchMouseUpEventName,n),document.removeEventListener(e.touchMouseMoveEventName,s)}))}updateBackground(t){}dispose(){o(this.container)}}class c extends h{constructor(t,e){super(t,e)}getIndicatorCssCelector(){return".dx-blazor-colorpicker-hue-selection-rect"}moveIndicator(e){let r=n(t.getEventY(e),this.indicator,!1);r=Math.min(this.getContainerHeight()-Math.round(this.getIndicatorHeight()/2),Math.max(0-this.getIndicatorHeight()/2,r)),this.indicator.style.top=r+"px";const o={hue:this.getHueValue(r),saturation:s.maxHsb.saturation,brightness:s.maxHsb.brightness};this.indicator.style.backgroundColor=s.hsbToHtml(o),this.onColorChanged(o)}updateColor(t){this.indicator.style.top=this.calculateHueIndicatorPosition(t.hue)+"px",this.indicator.style.backgroundColor=s.hsbToHtml({hue:t.hue,saturation:s.maxHsb.saturation,brightness:s.maxHsb.brightness})}calculateHueIndicatorPosition(t){const e=this.getContainerHeight(),r=this.getIndicatorHeight();let o=e-e*t/s.maxHsb.hue;const i=Math.round(r/2);return o=Math.min(e-i,Math.max(0-r/2,o-i)),Math.round(o)}getHueValue(t){let e=(t+=t<0?this.getIndicatorHeight()/2:Math.round(this.getIndicatorHeight()/2))*s.maxHsb.hue/this.getContainerHeight();return e=s.maxHsb.hue-e,Math.round(e)}}class u extends h{constructor(t,e){super(t,e)}getIndicatorCssCelector(){return".dx-blazor-colorpicker-color-selection"}moveIndicator(e){let r=n(t.getEventX(e),this.indicator,!0),o=n(t.getEventY(e),this.indicator,!1);r=Math.min(this.getContainerWidth(),Math.max(0,r))-this.getIndicatorWidth()/2,o=Math.min(this.getContainerHeight(),Math.max(0,o))-this.getIndicatorHeight()/2,this.indicator.style.top=o+"px",this.indicator.style.left=r+"px";const i={hue:s.maxHsb.hue,saturation:this.getSaturationValue(r),brightness:this.getBrightnessValue(o)};this.onColorChanged(i)}updateColor(t){this.setIndicatorPosition(t),this.updateBackground(t)}updateBackground(t){this.container.style.backgroundColor=s.hsbToHtml({hue:t.hue,saturation:s.maxHsb.saturation,brightness:s.maxHsb.brightness}),this.indicator.style.backgroundColor=s.hsbToHtml(t)}setIndicatorPosition(t){this.indicator.style.left=this.convertSaturationToXCoordinate(t.saturation)+"px",this.indicator.style.top=this.convertBrightnessToYCoordinate(t.brightness)+"px"}convertSaturationToXCoordinate(t){const e=this.getContainerWidth()*t/s.maxHsb.saturation-this.getIndicatorWidth()/2;return Math.floor(e)}convertBrightnessToYCoordinate(t){const e=this.getContainerHeight(),r=e-e*t/s.maxHsb.brightness-this.getIndicatorHeight()/2;return Math.floor(r)}getSaturationValue(t){const e=(t+this.getIndicatorWidth()/2)*s.maxHsb.saturation/this.getContainerWidth();return Math.floor(e)}getBrightnessValue(t){const e=s.maxHsb.brightness-(t+this.getIndicatorHeight()/2)*s.maxHsb.brightness/this.getContainerHeight();return Math.floor(e)}}const d=new Map;function g(t,e,r){if(!(t=i(t)))return;let o=d.get(t);o||(o=new a(t,r),d.set(t,o)),o.updateColor(e)}function l(t,e){if(!(t=i(t)))return;const r=d.get(t);r&&r.updateColor(e)}function b(t){if(!(t=i(t)))return;const e=d.get(t);e&&(e.dispose(),d.delete(t))}const m={initialize:g,updateColor:l,dispose:b};export default m;export{b as dispose,g as initialize,l as updateColor};
