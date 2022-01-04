(const ComponentsWindowName = "DxBlazor";
const DxBlazorModuleName = "dx-blazor-DX_BLAZOR_MODULE_NAME_PLACEHOLDER.js";
const Host = window;
const UserAgent = window.navigator.userAgent;
const DeviceInfo = {
    isMobileDevice: /(cpu iphone os)|(Android)/i.test(UserAgent),
    isTablet: /(ipad)/i.test(UserAgent)
}

async function initializeEnvironment() {
    if(Host[ComponentsWindowName])
        return;

    try {
        const module = await import("./modules/" + DxBlazorModuleName);
        Object.defineProperty(Host, ComponentsWindowName, {
            value: module.default
        });
    } catch (e) {
        Host.console.error(e);
    }
}

export function getDeviceInfo() {
    return DeviceInfo;
}
)("dx-blazor-32f20536.js", this.DxBlazorInternal);