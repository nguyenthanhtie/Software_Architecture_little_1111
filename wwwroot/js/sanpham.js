// JS cho nút Thêm vào giỏ ở giao diện sản phẩm

document.addEventListener('DOMContentLoaded', function () {
    // Lắng nghe sự kiện click cho tất cả nút thêm vào giỏ
    document.querySelectorAll('.btn-add-cart').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            var productId = btn.getAttribute('data-id');
            var quantityInput = document.getElementById('productQuantity_' + productId);
            var quantity = quantityInput ? parseInt(quantityInput.value) : 1;
            if (!productId) return;
            fetch('/KhachHang/Cart/AddToCart', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                },
                body: JSON.stringify({ id: productId, soLuong: quantity })
            })
            .then(response => response.json())
            .then(data => {
                showToast('Đã thêm vào giỏ hàng!');
                updateCartCount(data.cartCount);
            })
            .catch(() => {
                showToast('Có lỗi xảy ra, vui lòng thử lại!', true);
            });
        });
    });
});

function showToast(message, isError) {
    var toast = document.createElement('div');
    toast.className = 'toast-message' + (isError ? ' error' : '');
    toast.innerText = message;
    document.body.appendChild(toast);
    setTimeout(function () {
        toast.remove();
    }, 2000);
}

function updateCartCount(count) {
    var cartCount = document.getElementById('cart-count');
    if (cartCount) {
        cartCount.innerText = count;
    }
}

// CSS toast-message: nên thêm vào file css
// .toast-message { position: fixed; top: 20px; right: 20px; background: #28a745; color: #fff; padding: 10px 20px; border-radius: 5px; z-index: 9999; box-shadow: 0 2px 8px rgba(0,0,0,0.2); font-size: 16px; }
// .toast-message.error { background: #dc3545; }
