$(document).ready(function() {
    var currentCategoryId = null;

    // Initialize sortable
    $('#categoryTableBody').sortable({
        handle: '.sortable-handle',
        placeholder: 'sortable-placeholder',
        helper: function(e, tr) {
            var $originals = tr.children();
            var $helper = tr.clone();
            $helper.children().each(function(index) {
                $(this).width($originals.eq(index).width());
            });
            return $helper;
        },
        start: function(e, ui) {
            ui.placeholder.html('<td colspan="6"></td>');
        },
        update: function(e, ui) {
            updateCategoryOrder();
        }
    });

    // Search functionality
    $('#searchInput').on('keyup', function() {
        var value = $(this).val().toLowerCase();
        $('#categoryTable tbody tr').filter(function() {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });

    // Click on row to view products
    $('.clickable-row').click(function() {
        var categoryId = $(this).data('category-id');
        var categoryName = $(this).data('category-name');
        loadProductsForCategory(categoryId, categoryName);
    });

    // View products button
    $('.view-products').click(function() {
        var categoryId = $(this).data('category-id');
        var categoryName = $(this).data('category-name');
        loadProductsForCategory(categoryId, categoryName);
    });

    // Add category form
    $('#addCategoryForm').submit(function(e) {
        e.preventDefault();
        
        var name = $('#categoryName').val().trim();
        var description = $('#categoryDescription').val().trim();
        
        if (!name) {
            $('#categoryName').addClass('is-invalid');
            return;
        }
        
        $('#categoryName').removeClass('is-invalid');
        
        var data = {
            TenDanhMuc: name,
            MoTa: description || null
        };
        
        $.ajax({
            url: window.danhmucUrls.create,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function(response) {
                if (response.success) {
                    $('#addCategoryModal').modal('hide');
                    showToast('Thêm danh mục thành công!', 'success');
                    setTimeout(() => location.reload(), 1000);
                } else {
                    showToast('Có lỗi xảy ra: ' + response.message, 'error');
                }
            },
            error: function() {
                showToast('Có lỗi xảy ra khi thêm danh mục', 'error');
            }
        });
    });

    // Edit category form
    $('#editCategoryForm').submit(function(e) {
        e.preventDefault();
        
        var id = $('#editCategoryId').val();
        var name = $('#editCategoryName').val().trim();
        var description = $('#editCategoryDescription').val().trim();
        
        if (!name) {
            $('#editCategoryName').addClass('is-invalid');
            return;
        }
        
        $('#editCategoryName').removeClass('is-invalid');
        
        var data = {
            Id: parseInt(id),
            TenDanhMuc: name,
            MoTa: description || null
        };
        
        $.ajax({
            url: window.danhmucUrls.update,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function(response) {
                if (response.success) {
                    $('#editCategoryModal').modal('hide');
                    showToast('Cập nhật danh mục thành công!', 'success');
                    setTimeout(() => location.reload(), 1000);
                } else {
                    showToast('Có lỗi xảy ra: ' + response.message, 'error');
                }
            },
            error: function() {
                showToast('Có lỗi xảy ra khi cập nhật danh mục', 'error');
            }
        });
    });

    // Reset forms when modals are hidden
    $('#addCategoryModal').on('hidden.bs.modal', function() {
        $('#addCategoryForm')[0].reset();
        $('#categoryName').removeClass('is-invalid');
    });

    $('#editCategoryModal').on('hidden.bs.modal', function() {
        $('#editCategoryForm')[0].reset();
        $('#editCategoryName').removeClass('is-invalid');
    });

    // Handle confirm delete button click
    $(document).on('click', '#confirmDeleteBtn', function() {
        var categoryId = $(this).data('category-id');
        $('#confirmDeleteModal').modal('hide');
        deleteCategory(categoryId);
    });

    // Inline edit category name events
    $(document).on('click', '#categoryNameDisplay', function() {
        enterEditMode('name');
    });

    $(document).on('click', '#categoryDescDisplay', function() {
        enterEditMode('description');
    });

    $(document).on('keypress', '#categoryNameEdit', function(e) {
        if (e.which == 13) { // Enter key
            saveCategoryName();
        }
    });

    $(document).on('keydown', '#categoryNameEdit', function(e) {
        if (e.which == 27) { // Escape key
            resetEditMode();
        }
    });

    $(document).on('keypress', '#categoryDescEdit', function(e) {
        if (e.which == 13) { // Enter key
            saveCategoryDescription();
        }
    });

    $(document).on('keydown', '#categoryDescEdit', function(e) {
        if (e.which == 27) { // Escape key
            resetEditMode();
        }
    });

    // Delete category from modal
    $(document).on('click', '#deleteCategoryBtn', function() {
        var categoryId = $('#currentCategoryId').val();
        var categoryName = $('#categoryNameDisplay').text();
        var productCount = parseInt($(this).attr('data-product-count')) || 0;
        
        // Close the products modal first
        $('#viewProductsModal').modal('hide');
        
        // Show confirmation modal
        showConfirmDeleteModal(categoryId, categoryName, productCount);
    });
});

function deleteCategory(categoryId) {
    $.ajax({
        url: window.danhmucUrls.delete,
        type: 'POST',
        data: { id: categoryId },
        success: function(response) {
            if (response.success) {
                showToast('Xóa danh mục thành công!', 'success');
                // Close modal if it's open
                $('#viewProductsModal').modal('hide');
                setTimeout(() => location.reload(), 1000);
            } else {
                showToast('Có lỗi xảy ra: ' + response.message, 'error');
            }
        },
        error: function() {
            showToast('Có lỗi xảy ra khi xóa danh mục', 'error');
        }
    });
}

function updateCategoryOrder() {
    var categoryIds = [];
    $('#categoryTableBody tr').each(function(index) {
        var categoryId = $(this).data('category-id');
        if (categoryId) {
            categoryIds.push(categoryId);
            // Update display order number in the table
            $(this).find('td:eq(1)').text(index + 1);
        }
    });

    if (categoryIds.length > 0) {
        $.ajax({
            url: window.danhmucUrls.updateOrder,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(categoryIds),
            success: function(response) {
                if (response.success) {
                    showToast('Cập nhật thứ tự thành công!', 'success', 2000);
                } else {
                    showToast('Có lỗi xảy ra khi cập nhật thứ tự: ' + response.message, 'error');
                    location.reload(); // Reload to restore original order
                }
            },
            error: function() {
                showToast('Có lỗi xảy ra khi cập nhật thứ tự', 'error');
                location.reload(); // Reload to restore original order
            }
        });
    }
}

// Custom notification functions
function showToast(message, type = 'info', duration = 5000) {
    const toastId = 'toast-' + Date.now();
    const iconMap = {
        success: 'fas fa-check-circle',
        error: 'fas fa-exclamation-circle',
        warning: 'fas fa-exclamation-triangle',
        info: 'fas fa-info-circle'
    };
    
    const bgMap = {
        success: 'bg-success',
        error: 'bg-danger',
        warning: 'bg-warning',
        info: 'bg-primary'
    };
    
    const toast = $(`
        <div id="${toastId}" class="toast align-items-center text-white ${bgMap[type]} border-0 mb-2" role="alert">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="${iconMap[type]} me-2"></i>${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `);
    
    // Create toast container if it doesn't exist
    if ($('#toast-container').length === 0) {
        $('body').append('<div id="toast-container" style="position: fixed; top: 20px; right: 20px; z-index: 9999;"></div>');
    }
    
    $('#toast-container').append(toast);
    
    // Initialize and show toast
    const bsToast = new bootstrap.Toast(toast[0], {
        autohide: true,
        delay: duration
    });
    bsToast.show();
    
    // Remove toast element after it's hidden
    toast.on('hidden.bs.toast', function() {
        $(this).remove();
    });
}

function showConfirmDeleteModal(categoryId, categoryName, productCount) {
    $('#deleteCategoryName').text(categoryName);
    $('#confirmDeleteBtn').data('category-id', categoryId);
    
    if (productCount > 0) {
        $('#productCount').text(productCount);
        $('#deleteWarning').removeClass('d-none');
        $('#confirmDeleteBtn').prop('disabled', true).text('Không thể xóa');
    } else {
        $('#deleteWarning').addClass('d-none');
        $('#confirmDeleteBtn').prop('disabled', false).html('<i class="fas fa-trash me-1"></i>Xóa danh mục');
    }
    
    $('#confirmDeleteModal').modal('show');
}

function loadProductsForCategory(categoryId, categoryName) {
    // Set category name in modal
    $('#categoryNameDisplay').text(categoryName);
    $('#currentCategoryId').val(categoryId);
    
    // Load category description
    loadCategoryDescription(categoryId);
    
    // Reset edit mode
    resetEditMode();
    
    // Show loading state
    $('#productsList').html(`
        <div class="text-center">
            <i class="fas fa-spinner fa-spin fa-2x text-muted"></i>
            <p class="mt-2">Đang tải danh sách sản phẩm...</p>
        </div>
    `);
    
    // Show modal
    $('#viewProductsModal').modal('show');
    
    // Load products via AJAX
    $.ajax({
        url: window.danhmucUrls.getCategoryProducts,
        type: 'GET',
        data: { id: categoryId },
        success: function(products) {
            console.log('Products data:', products); // Debug log
            
            // Store product count for delete functionality
            $('#deleteCategoryBtn').attr('data-product-count', products ? products.length : 0);
            
            if (products && products.length > 0) {
                let html = '<div class="row">';
                products.forEach(function(product) {
                    console.log('Product item:', product); // Debug log
                    html += `
                        <div class="col-md-4 col-lg-3 mb-3">
                            <div class="card h-100 product-card">
                                <div class="position-relative">
                                    <img src="${product.mainImage || '/Images/noimage.jpg'}" 
                                         class="card-img-top product-image" 
                                         alt="${product.name || 'Sản phẩm'}"
                                         style="height: 140px; object-fit: cover;"
                                         onerror="this.src='/Images/noimage.jpg'">
                                    <span class="badge ${product.status === 'Hiển thị' ? 'bg-success' : 'bg-secondary'} position-absolute top-0 end-0 m-2">
                                        ${product.status || 'Không xác định'}
                                    </span>
                                </div>
                                <div class="card-body d-flex flex-column">
                                    <h6 class="card-title text-truncate" title="${product.name || 'Chưa có tên'}">
                                        ${product.name || 'Chưa có tên'}
                                    </h6>
                                    <div class="d-flex justify-content-between align-items-end mt-auto">
                
                                        <div class="product-price">
                                            <p class="card-text text-primary fw-bold mb-0">
                                                ${product.price || '0 ₫'}
                                            </p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    `;
                });
                html += '</div>';
                $('#productsList').html(html);
            } else {
                $('#productsList').html(`
                    <div class="text-center py-4">
                        <i class="fas fa-box-open fa-3x text-muted mb-3"></i>
                        <h5 class="text-muted">Danh mục chưa có sản phẩm nào</h5>
                        <p class="text-muted">Thêm sản phẩm vào danh mục để hiển thị ở đây</p>
                    </div>
                `);
            }
        },
        error: function() {
            $('#productsList').html(`
                <div class="text-center py-4">
                    <i class="fas fa-exclamation-triangle fa-3x text-danger mb-3"></i>
                    <h5 class="text-danger">Có lỗi xảy ra</h5>
                    <p class="text-muted">Không thể tải danh sách sản phẩm</p>
                </div>
            `);
        }
    });
}

// Add new function to load category description
function loadCategoryDescription(categoryId) {
    // Find the category row to get description
    var $categoryRow = $(`tr[data-category-id="${categoryId}"]`);
    var description = $categoryRow.find('td:nth-child(4) span').text();
    
    if (description && description !== 'Chưa có mô tả') {
        $('#categoryDescDisplay').text(description);
    } else {
        $('#categoryDescDisplay').text('Chưa có mô tả');
    }
}

function enterEditMode(type) {
    // Reset any existing edit mode first
    resetEditMode();
    
    if (type === 'name') {
        var currentName = $('#categoryNameDisplay').text();
        $('#categoryNameEdit').val(currentName);
        $('#categoryNameDisplay').addClass('d-none');
        $('#categoryNameEdit').removeClass('d-none');
        $('#categoryNameEdit').focus().select();
    } else if (type === 'description') {
        var currentDesc = $('#categoryDescDisplay').text();
        $('#categoryDescEdit').val(currentDesc === 'Chưa có mô tả' ? '' : currentDesc);
        $('#categoryDescDisplay').addClass('d-none');
        $('#categoryDescEdit').removeClass('d-none');
        $('#categoryDescEdit').focus().select();
    }
}

function resetEditMode() {
    $('#categoryNameDisplay').removeClass('d-none');
    $('#categoryDescDisplay').removeClass('d-none');
    $('#categoryNameEdit').addClass('d-none');
    $('#categoryDescEdit').addClass('d-none');
}

function saveCategoryName() {
    var categoryId = $('#currentCategoryId').val();
    var newName = $('#categoryNameEdit').val().trim();
    var originalName = $('#categoryNameDisplay').text();
    
    if (!newName) {
        showToast('Tên danh mục không được để trống', 'error');
        $('#categoryNameEdit').focus();
        return;
    }
    
    // Check if anything changed
    if (newName === originalName) {
        resetEditMode();
        return;
    }
    
    $.ajax({
        url: window.danhmucUrls.update,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            Id: parseInt(categoryId),
            TenDanhMuc: newName,
            MoTa: null // Don't update description when editing name
        }),
        success: function(response) {
            if (response.success) {
                $('#categoryNameDisplay').text(newName);
                resetEditMode();
                showToast('Cập nhật tên danh mục thành công!', 'success');
                
                // Update the name in the main table
                $(`tr[data-category-id="${categoryId}"]`).attr('data-category-name', newName);
                $(`tr[data-category-id="${categoryId}"] td:nth-child(3) strong`).text(newName);
            } else {
                showToast('Có lỗi xảy ra: ' + response.message, 'error');
            }
        },
        error: function() {
            showToast('Có lỗi xảy ra khi cập nhật tên danh mục', 'error');
        }
    });
}

function saveCategoryDescription() {
    var categoryId = $('#currentCategoryId').val();
    var newDesc = $('#categoryDescEdit').val().trim();
    var originalDesc = $('#categoryDescDisplay').text();
    var currentName = $('#categoryNameDisplay').text();
    
    // Check if anything changed
    if (newDesc === originalDesc || (newDesc === '' && originalDesc === 'Chưa có mô tả')) {
        resetEditMode();
        return;
    }
    
    $.ajax({
        url: window.danhmucUrls.update,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            Id: parseInt(categoryId),
            TenDanhMuc: currentName, // Keep current name
            MoTa: newDesc || null
        }),
        success: function(response) {
            if (response.success) {
                $('#categoryDescDisplay').text(newDesc || 'Chưa có mô tả');
                resetEditMode();
                showToast('Cập nhật mô tả danh mục thành công!', 'success');
                
                // Update the description in the main table
                $(`tr[data-category-id="${categoryId}"] td:nth-child(4) span`).text(newDesc || 'Chưa có mô tả');
            } else {
                showToast('Có lỗi xảy ra: ' + response.message, 'error');
            }
        },
        error: function() {
            showToast('Có lỗi xảy ra khi cập nhật mô tả danh mục', 'error');
        }
    });
}
    