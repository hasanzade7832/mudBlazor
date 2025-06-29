/* ---------------- wwwroot/js/navMenu.js ---------------- */
window.registerOutsideClick = (dotnetHelper) => {
    function handler(e) {
        // اگر روی المنتی با data-ignore کلیک شد نادیده بگیر
        if (e.target.closest('[data-ignore-outside]')) return;
        dotnetHelper.invokeMethodAsync('CloseDropdown');
    }
    document.addEventListener('mousedown', handler);

    // Blazor می‌تواند در آینده دستور حذف بدهد
    return {
        dispose: () => document.removeEventListener('mousedown', handler)
    };
};
