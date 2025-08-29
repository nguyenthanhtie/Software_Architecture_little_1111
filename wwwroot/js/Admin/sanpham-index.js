// Quản lý sản phẩm - JavaScript (Simplified for current database structure)
$(document).ready(function() {
    // Khởi tạo Bootstrap tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl, {
            trigger: 'hover',
            delay: { show: 300, hide: 100 }
        });
    });
    
    // Xử lý click vào dòng sản phẩm hoặc nút xem chi tiết
    $(document).on('click', '.clickable-row, .view-product', function (e) {
        // Nếu là nút view-product thì ngăn sự kiện nổi bọt lên tr
        if ($(e.target).closest('.view-product').length > 0) {
            e.stopPropagation();
        }
        var id = $(this).data('id') || $(this).closest('tr').data('id');
        if (!id) return;
        $('#productDetailModal').modal('show');
        $('#productDetailContent').html('<div class="text-center py-5"><div class="spinner-border text-primary" role="status"></div></div>');
        
        var detailUrl = window.productUrls.detail;
        $.get(detailUrl, { id: id }, function (data) {
            $('#productDetailContent').html(data);
        }).fail(function () {
            $('#productDetailContent').html('<div class="alert alert-danger">Không thể tải chi tiết sản phẩm.</div>');
        });
    });

    function loadProductDetail(productId) {
        $('#productDetailContent').html('<div class="text-center py-5"><div class="spinner-border text-primary" role="status"></div></div>');
        
        $.get(window.productUrls.detail, { id: productId })
            .done(function(data) {
                $('#productDetailContent').html(data);
            })
            .fail(function() {
                $('#productDetailContent').html('<div class="alert alert-danger">Không thể tải chi tiết sản phẩm.</div>');
            });
    }

    // Mở modal Thêm sản phẩm
    $('#addProductBtn').on('click', function (e) {
        e.preventDefault();
        $('#addEditProductModalLabel').text('Thêm sản phẩm mới');
        $('#addEditProductModal').modal('show');
        loadProductForm();
    });

    // Hàm tải form sản phẩm
    function loadProductForm(id = null) {
        $('#addEditProductContent').html('<div class="text-center py-5"><div class="spinner-border text-primary" role="status"></div></div>');
        var url = window.productUrls.addOrEdit;
        if (id) url += '?id=' + id;
        
        $.get(url, function (data) {
            $('#addEditProductContent').html(data);
        }).fail(function () {
            $('#addEditProductContent').html('<div class="alert alert-danger">Không thể tải form.</div>');
        });
    }

    // Submit form Thêm/Chỉnh sửa sản phẩm qua AJAX - chỉ gắn khi cần thiết
    $(document).off('submit', '#addEditProductForm').on('submit', '#addEditProductForm', function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        
        var $form = $(this);
        var $submitBtn = $form.find('button[type="submit"]');
        
        // Kiểm tra nếu đang trong quá trình submit
        if ($submitBtn.prop('disabled') || $form.data('submitting')) {
            return false;
        }
        
        // Đánh dấu form đang submit
        $form.data('submitting', true);
        
        var formData = new FormData(this);
        
        // Thêm loading indicator
        var originalBtnText = $submitBtn.html();
        $submitBtn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Đang xử lý...');
        
        $.ajax({
            url: $form.attr('action'),
            type: $form.attr('method'),
            data: formData,
            processData: false,
            contentType: false,
            timeout: 30000,
            success: function (res) {
                if (res.success) {
                    // Thông báo thành công
                    showToast('Lưu sản phẩm thành công!', 'success');
                    $('#addEditProductModal').modal('hide');
                    setTimeout(() => location.reload(), 1000);
                } else {
                    // Hiển thị thông báo lỗi
                    if (res.message) {
                        showToast('Lỗi: ' + res.message, 'error');
                    } else if (res.html) {
                        // Cập nhật form với dữ liệu mới nếu có
                        $('#addEditProductContent').html(res.html);
                    } else {
                        showToast('Có lỗi xảy ra khi lưu sản phẩm!', 'error');
                    }
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                // Xử lý lỗi AJAX
                var errorMessage = 'Có lỗi xảy ra khi gửi form!';
                if (textStatus === 'timeout') {
                    errorMessage = 'Yêu cầu quá thời gian chờ. Vui lòng thử lại.';
                } else if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMessage = xhr.responseJSON.message;
                }
                
                showToast('Lỗi: ' + errorMessage, 'error');
                console.error('AJAX Error:', xhr);
            },
            complete: function() {
                // Reset trạng thái form
                $form.data('submitting', false);
                $submitBtn.prop('disabled', false).html(originalBtnText);
            }
        });
        
        return false;
    });
    
    // Xử lý nút chỉnh sửa trong modal chi tiết
    $(document).on('click', '.edit-product-detail', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $('#productDetailModal').modal('hide');
        
        // Mở modal chỉnh sửa
        setTimeout(() => {
            $('#addEditProductModalLabel').text('Chỉnh sửa sản phẩm');
            $('#addEditProductModal').modal('show');
            loadProductForm(id);
        }, 300);
    });

    // Xử lý nút xóa trong modal chi tiết
    $(document).on('click', '.delete-product-detail', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var $btn = $(this);
        
        // Lấy tên sản phẩm để hiển thị trong thông báo
        var productName = $('#productDetailModal .modal-body h4').first().text() || 'sản phẩm này';
        
        // Hiển thị modal xác nhận xóa
        showConfirmModal(
            'Xác nhận xóa sản phẩm',
            `Bạn có chắc chắn muốn XÓA VĨNH VIỄN sản phẩm "${productName}"?<br><br>
            <div class="alert alert-warning mb-0">
                <strong>⚠️ CẢNH BÁO:</strong><br>
                • Sản phẩm sẽ bị xóa hoàn toàn khỏi hệ thống<br>
                • Tất cả ảnh và thông tin liên quan sẽ bị xóa<br>
                • Thao tác này KHÔNG THỂ HOÀN TÁC!
            </div>`,
            function() {
                $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-2"></span>Đang xóa...');
                
                $.ajax({
                    url: window.productUrls.delete,
                    type: 'POST',
                    data: {
                        id: id,
                        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                    },
                    timeout: 30000,
                    success: function (response) {
                        if (response.success) {
                            showToast('Xóa sản phẩm thành công!', 'success');
                            $('#productDetailModal').modal('hide');
                            setTimeout(() => location.reload(), 1000);
                        } else {
                            showToast('Lỗi: ' + (response.message || 'Không thể xóa sản phẩm'), 'error');
                            $btn.prop('disabled', false).html('<i class="fas fa-trash me-1"></i> Xóa vĩnh viễn');
                        }
                    },
                    error: function (xhr, textStatus) {
                        var errorMsg = 'Lỗi kết nối máy chủ';
                        if (textStatus === 'timeout') {
                            errorMsg = 'Yêu cầu quá thời gian chờ. Vui lòng thử lại.';
                        } else if (xhr.responseJSON?.message) {
                            errorMsg = xhr.responseJSON.message;
                        }
                        showToast('Lỗi: ' + errorMsg, 'error');
                        $btn.prop('disabled', false).html('<i class="fas fa-trash me-1"></i> Xóa vĩnh viễn');
                    }
                });
            }
        );
    });
    
    $('#refreshPageBtn').on('click', function() {
        location.reload();
    });
});

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

function showConfirmModal(title, message, onConfirm, onCancel = null) {
    const modalId = 'confirmModal-' + Date.now();
    const modal = $(`
        <div class="modal fade" id="${modalId}" tabindex="-1" data-bs-backdrop="static">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content border-0 shadow-lg">
                    <div class="modal-header bg-light border-0 pb-2">
                        <h5 class="modal-title fw-bold text-dark">
                            <i class="fas fa-exclamation-triangle text-warning me-2"></i>${title}
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Đóng"></button>
                    </div>
                    <div class="modal-body px-4 py-3">
                        ${message}
                    </div>
                    <div class="modal-footer bg-light border-0 pt-2">
                        <button type="button" class="btn btn-light border me-2" data-bs-dismiss="modal">
                            <i class="fas fa-times me-1"></i>Hủy bỏ
                        </button>
                        <button type="button" class="btn btn-danger confirm-btn">
                            <i class="fas fa-trash me-1"></i>Xác nhận xóa
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `);
    
    $('body').append(modal);
    
    // Handle confirm button click
    modal.find('.confirm-btn').on('click', function() {
        modal.modal('hide');
        if (onConfirm) onConfirm();
    });
    
    // Handle cancel
    modal.on('hidden.bs.modal', function() {
        $(this).remove();
        if (onCancel) onCancel();
    });
    
    // Show modal
    modal.modal('show');
}
