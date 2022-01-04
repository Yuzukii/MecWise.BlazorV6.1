(const ComponentsWindowName = "DxBlazor";
const DxBlazorModuleName = "dx-blazor-DX_BLAZOR_MODULE_NAME_PLACEHOLDER.js";
function createDeviceInfoAndDefineEntryPoint(module) {
    window.DxBlazor = module.default;
    const UserAgent = window.navigator.userAgent;
    return { isMobileDevice: /(cpu iphone os)|(Android)/i.test(UserAgent), isTablet: /(ipad)/i.test(UserAgent) };
};
const deviceInfoAsync = import("./modules/" + DxBlazorModuleName).then(createDeviceInfoAndDefineEntryPoint);
async function getDeviceInfo() {
    return await deviceInfoAsync;
}
)("dx-blazor-750968d4.js", this.DxBlazorInternal);