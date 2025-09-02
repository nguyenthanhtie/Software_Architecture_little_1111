$(document).ready(function() {
    // Search functionality (fuzzy, case-insensitive)
    $('#searchInput').on('keyup', function() {
        var value = $(this).val().toLowerCase();
        $('#orderTable tbody tr').each(function() {
            var rowText = $(this).text().toLowerCase();
            if (fuzzyMatch(rowText, value)) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    });

    // Simple fuzzy match: returns true if all characters in pattern appear in order in text (not necessarily consecutively)
    function fuzzyMatch(text, pattern) {
        if (!pattern) return true;
        var t = 0, p = 0;
        while (t < text.length && p < pattern.length) {
            if (text[t] === pattern[p]) p++;
            t++;
        }
        return p === pattern.length;
    }

    // Status filter
    $('#statusFilter').change(function() {
        var selectedStatus = $(this).val().toLowerCase();
        $('#orderTable tbody tr').each(function() {
            var status = $(this).find('td:eq(4)').text().toLowerCase();
            if (selectedStatus === '' || status.indexOf(selectedStatus) > -1) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    });

    // Click on row to view order detail
    $('.order-row').click(function() {
        var orderId = $(this).data('order-id');
        loadOrderDetail(orderId);
    });

    // Add hover effect for better UX
    $('.order-row').hover(
        function() {
            $(this).addClass('table-active');
        },
        function() {
            $(this).removeClass('table-active');
        }
    );

    // Click on status badge to cycle through statuses
    $('.status-badge').click(function(e) {
        e.stopPropagation(); // Prevent row click
        var orderId = $(this).data('order-id');
        var currentStatus = $(this).text().trim();
        
        // Không cho phép thay đổi trạng thái nếu đã hủy hoặc hoàn thành
        if (currentStatus === 'Đã hủy') {
            alert('Đơn hàng đã hủy không thể thay đổi trạng thái');
            return;
        }
        
        if (currentStatus === 'Hoàn thành') {
            alert('Đơn hàng đã hoàn thành không thể thay đổi trạng thái');
            return;
        }
        
        // Hiển thị menu tùy chọn cho trạng thái
        showStatusMenu(e, orderId, currentStatus);
    });
});

function loadOrderDetail(orderId) {
    // Show loading state
    $('#orderModalId').text(orderId);
    $('#orderCustomerName').text('Đang tải...');
    $('#orderCustomerEmail').text('Đang tải...');
    $('#orderCustomerPhone').text('Đang tải...');
    $('#orderAddress').text('Đang tải...');
    $('#orderDate').text('Đang tải...');
    $('#orderPaymentMethod').text('Đang tải...');
    $('#orderTotal').text('Đang tải...');
    
    var itemsList = $('#orderItemsList');
    itemsList.html('<tr><td colspan="5" class="text-center">Đang tải chi tiết sản phẩm...</td></tr>');
    
    $('#orderDetailModal').modal('show');
    
    // Load order details from server
    $.ajax({
        url: window.donhangUrls.getOrderDetail,
        type: 'GET',
        data: { id: orderId },
        success: function(data) {
            // Update modal content
            $('#orderModalId').text(data.id);
            $('#orderCustomerName').text(data.customerName);
            $('#orderCustomerEmail').text(data.customerEmail);
            $('#orderCustomerPhone').text(data.customerPhone);
            $('#orderAddress').text(data.address);
            $('#orderDate').text(data.orderDate);
            $('#orderPaymentMethod').text(data.paymentMethod);
            $('#orderTotal').text(data.totalAmount);
            
            // Update status badge
            var statusBadge = $('#orderStatus');
            statusBadge.removeClass().addClass('badge ' + getStatusClass(data.status));
            statusBadge.text(data.status);
            
            // Update items list
            var itemsHtml = '';
            if (data.items && data.items.length > 0) {
                data.items.forEach(function(item) {
                    itemsHtml += '<tr>' +
                        '<td>' + item.productName + '</td>' +
                        '<td>' + item.variantSku + '</td>' +
                        '<td>' + item.quantity + '</td>' +
                        '<td>' + item.price + '</td>' +
                        '<td>' + item.subTotal + '</td>' +
                        '</tr>';
                });
            } else {
                itemsHtml = '<tr><td colspan="5" class="text-center">Không có sản phẩm nào</td></tr>';
            }
            itemsList.html(itemsHtml);
        },
        error: function() {
            alert('Không thể tải thông tin đơn hàng');
        }
    });
}

function showStatusMenu(event, orderId, currentStatus) {
    // Xóa menu cũ nếu có
    $('.status-menu').remove();
    
    // Lấy các trạng thái có thể chuyển đổi
    var availableStatuses = getAvailableStatuses(currentStatus);
    
    if (availableStatuses.length === 0) {
        alert('Không thể thay đổi trạng thái từ: ' + currentStatus);
        return;
    }
    
    // Tạo menu
    var menu = $('<div class="status-menu" style="position: absolute; background: white; border: 1px solid #ccc; border-radius: 4px; box-shadow: 0 2px 8px rgba(0,0,0,0.15); z-index: 1000; min-width: 150px;"></div>');
    
    availableStatuses.forEach(function(status) {
        var item = $('<div style="padding: 8px 12px; cursor: pointer; border-bottom: 1px solid #eee;" class="status-menu-item">' + status + '</div>');
        item.hover(
            function() { $(this).css('background-color', '#f8f9fa'); },
            function() { $(this).css('background-color', 'white'); }
        );
        item.click(function() {
            updateOrderStatus(orderId, status);
            $('.status-menu').remove();
        });
        menu.append(item);
    });
    
    // Đặt vị trí menu
    menu.css({
        left: event.pageX,
        top: event.pageY + 10
    });
    
    $('body').append(menu);
    
    // Đóng menu khi click bên ngoài
    $(document).one('click', function() {
        $('.status-menu').remove();
    });
}

function getAvailableStatuses(currentStatus) {
    var statusOptions = {
        'Đang xử lý': ['Đã xác nhận', 'Đã hủy'],
        'Đã xác nhận': ['Đang giao', 'Đã hủy'],
        'Đang giao': ['Hoàn thành', 'Đã hủy'],
        'Hoàn thành': [],
        'Đã hủy': []
    };
    
    return statusOptions[currentStatus] || [];
}

function getNextStatus(currentStatus) {
    // Định nghĩa flow trạng thái hợp lệ - giữ để tương thích
    var statusFlow = {
        'Đang xử lý': 'Đã xác nhận',
        'Đã xác nhận': 'Đang giao', 
        'Đang giao': 'Hoàn thành',
        'Hoàn thành': null, // Không thể chuyển đổi từ hoàn thành
        'Đã hủy': null // Không thể chuyển đổi từ đã hủy
    };
    
    // Trả về trạng thái tiếp theo hoặc null nếu không thể chuyển đổi
    return statusFlow[currentStatus] || null;
}

function updateOrderStatus(orderId, status) {
    $.ajax({
        url: window.donhangUrls.updateStatus,
        type: 'POST',
        data: { id: orderId, status: status },
        success: function(response) {
            if (response.success) {
                // Update the status badge immediately without page reload
                var badge = $('.status-badge[data-order-id="' + orderId + '"]');
                badge.removeClass().addClass('badge status-badge ' + getStatusClass(status));
                badge.text(status);
                badge.attr('title', 'Click để thay đổi trạng thái');
                badge.css('cursor', 'pointer');
                
                // Show success message briefly
                var originalText = badge.text();
                badge.text('✓ Đã cập nhật');
                setTimeout(function() {
                    badge.text(originalText);
                }, 1000);
            } else {
                alert('Có lỗi xảy ra: ' + response.message);
            }
        },
        error: function() {
            alert('Có lỗi xảy ra khi cập nhật trạng thái');
        }
    });
}

function getStatusClass(status) {
    switch(status) {
        case 'Đang xử lý': return 'bg-warning text-dark';
        case 'Đã xác nhận': return 'bg-info text-white';
        case 'Đang giao': return 'bg-primary text-white';
        case 'Hoàn thành': return 'bg-success text-white';
        case 'Đã hủy': return 'bg-danger text-white';
        default: return 'bg-secondary text-white';
    }
}
