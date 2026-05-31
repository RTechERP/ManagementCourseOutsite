// Menu Script - Multi-expand

document.addEventListener('DOMContentLoaded', function() {
    initMultiExpand();
    highlightCurrentMenuItem();
});

// Chạy sau cùng khi page load xong
window.addEventListener('load', function() {
    setTimeout(function() {
        initMultiExpand();
    }, 100);
});

// Multi-expand: Cho phép mở nhiều nhóm cùng lúc
function initMultiExpand() {
    document.querySelectorAll('.menu-link.menu-toggle').forEach(toggle => {
        // Force mở tất cả
        toggle.classList.add('not-collapsed');
        
        // Toggle riêng lẻ khi click
        toggle.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            const subMenu = this.nextElementSibling;
            if (subMenu && subMenu.classList.contains('menu-sub')) {
                const isOpen = this.classList.contains('not-collapsed');
                
                if (isOpen) {
                    // Collapse
                    this.classList.remove('not-collapsed');
                    subMenu.style.display = 'none';
                } else {
                    // Expand
                    this.classList.add('not-collapsed');
                    subMenu.style.display = 'block';
                }
            }
        });
    });
    
    // Force hiển thị tất cả menu-sub
    document.querySelectorAll('.menu-sub').forEach(sub => {
        sub.style.display = 'block';
    });
}

// Highlight menu item hiện tại dựa trên URL
function highlightCurrentMenuItem() {
    const urlParams = new URLSearchParams(window.location.search);
    const currentCatalogId = urlParams.get('courseCatalogID');
    
    if (currentCatalogId) {
        const selectedItem = document.querySelector(`#menu_link_items_${currentCatalogId}`);
        if (selectedItem) {
            document.querySelectorAll('.menu-item').forEach(item => {
                item.classList.remove('active');
            });
            selectedItem.parentElement.classList.add('active');
            
            // Expand parent menu
            const parentSub = selectedItem.closest('.menu-sub');
            if (parentSub) {
                const parentToggle = parentSub.previousElementSibling;
                if (parentToggle && parentToggle.classList.contains('menu-link')) {
                    parentToggle.classList.add('not-collapsed');
                    parentSub.style.display = 'block';
                }
            }
        }
    }
}
