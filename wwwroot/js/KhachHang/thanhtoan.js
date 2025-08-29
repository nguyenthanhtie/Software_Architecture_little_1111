// Các biến DOM cần thiết (bỏ phần địa chỉ)
// const addAddressBtn = document.getElementById('addAddressBtn');
// const addressModal = document.getElementById('addressModal');
// const closeModalBtn = document.getElementById('closeModalBtn');
// const cancelAddressBtn = document.getElementById('cancelAddressBtn');
// const saveAddressBtn = document.getElementById('saveAddressBtn');
// const addressContainer = document.getElementById('addressList');

// Đảm bảo modal ẩn khi trang load
document.addEventListener('DOMContentLoaded', function() {
    console.log('=== THANH TOÁN JS LOADED ===', new Date().toISOString());
    
    // Đăng ký hàm processOrder để có thể gọi từ inline script
    window.processOrderFromJS = processOrderFromJS;
    
    // Không cần load địa chỉ nữa vì đã bỏ tính năng này
    // loadAddressesFromDatabase();
    
    // Kiểm tra xem dữ liệu đã được thiết lập từ inline script chưa
    if (window.pageLoaded && window.isBuyNow && window.buyNowData) {
        console.log('Buy Now data already available, initializing immediately...');
        initializeOrderDisplay();
    } else {
        // Đợi một chút để đảm bảo window.isBuyNow và window.buyNowData đã được thiết lập từ server
        setTimeout(() => {
            console.log('Initializing order display...');
            console.log('isBuyNow:', window.isBuyNow);
            console.log('buyNowData:', window.buyNowData);
            
            // Kiểm tra xem dữ liệu buyNow đã được thiết lập chưa
            if (window.isBuyNow && !window.buyNowData) {
                console.warn('isBuyNow is true but buyNowData is not available');
            }
            
            initializeOrderDisplay();
        }, 200); // Tăng thời gian chờ từ 100ms lên 200ms
    }
});

// Hàm đóng modal thành công - định nghĩa ngay từ đầu
function closeSuccessModal() {
  console.log('Đang đóng modal thành công...');
  const successModal = document.getElementById('paymentSuccessModal');
  if (successModal) {
    successModal.classList.remove('d-flex');
    successModal.classList.add('d-none');
    
    // Clear tất cả dữ liệu localStorage liên quan đến giỏ hàng (chỉ khi không phải "mua ngay")
    if (!window.isBuyNow) {
      localStorage.removeItem('orderItems');
      localStorage.removeItem('cartSelected');
      localStorage.removeItem('selectedCartItems');
    }
    
    // Chuyển hướng khác nhau tùy theo chế độ
    console.log('Đang chuyển hướng...');
    setTimeout(function() {
      if (window.isBuyNow) {
        // Nếu là "mua ngay", chuyển về trang chủ hoặc trang sản phẩm
        window.location.href = '/KhachHang/SanPham';
      } else {
        // Nếu là giỏ hàng thông thường, chuyển về giỏ hàng với refresh
        window.location.href = '/KhachHang/Cart?refresh=true';
      }
    }, 300);
  } else {
    console.error('Không tìm thấy modal thành công');
  }
}

// Hàm cập nhật số lượng giỏ hàng sau khi thanh toán thành công
async function updateCartCountAfterPayment() {
  try {
    console.log('Đang cập nhật số lượng giỏ hàng...');
    const response = await fetch('/KhachHang/Cart/GetCartCount', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    });
    
    if (response.ok) {
      const result = await response.json();
      const cartCountElement = document.getElementById('cartCount');
      
      if (cartCountElement) {
        if (result.cartCount > 0) {
          cartCountElement.textContent = result.cartCount;
          cartCountElement.style.display = '';
        } else {
          cartCountElement.style.display = 'none';
        }
        console.log(`Đã cập nhật số lượng giỏ hàng: ${result.cartCount}`);
      }
    } else {
      console.error('Lỗi khi lấy số lượng giỏ hàng:', response.status);
    }
  } catch (error) {
    console.error('Lỗi khi cập nhật số lượng giỏ hàng:', error);
  }
}

// Bỏ các event listener cho modal địa chỉ vì không còn cần thiết
// addAddressBtn?.addEventListener('click', () => {
//     addressModal.classList.remove('d-none');
//     addressModal.classList.add('d-flex');
// });

// [closeModalBtn, cancelAddressBtn].forEach(btn => {
//     btn?.addEventListener('click', () => {
//         addressModal.classList.remove('d-flex');
//         addressModal.classList.add('d-none');
//     });
// });

// Alert Modal System - Simple implementation using existing modal
function showAlertModal(title, message, icon = 'fas fa-exclamation-circle') {
    document.getElementById('alertModalTitle').textContent = title;
    document.getElementById('alertModalMessage').textContent = message;
    document.getElementById('alertModalIcon').className = icon;

    document.getElementById('alertModal').classList.remove('d-none');
    document.getElementById('alertModal').classList.add('d-flex');
}

function closeAlertModal() {
    const alertModal = document.getElementById('alertModal');
    alertModal.classList.remove('d-flex');
    alertModal.classList.add('d-none');
}

// Kiểm tra họ và tên: tối đa 50 ký tự, không ký tự đặc biệt, chữ cái đầu viết hoa, tiếng Việt có dấu
function validateFullName(name) {
  name = name.trim();
  if (name.length === 0) {
    showAlertModal("Lỗi Nhập Liệu", "Vui lòng nhập họ và tên", "fas fa-exclamation-triangle");
    return false;
  }
  if (name.length > 50) {
    showAlertModal("Lỗi Nhập Liệu", "Họ và tên không được vượt quá 50 ký tự", "fas fa-exclamation-triangle");
    return false;
  }
  // Cho phép chữ hoa tiếng Việt đầu từ, các ký tự còn lại là thường hoặc dấu nháy đơn, nhiều từ cách nhau bởi 1 khoảng trắng
  const regex = /^([A-ZÀÁÂÃÄÅĀĂĄẠẢẤẦẨẪẬẮẰẲẴẶÆÇĆĈĊČĐÐÈÉÊËĒĔĖĘĚẸẺẼẾỀỂỄỆÌÍÎÏĨĪĮİỈỊÑÒÓÔÕÖØŌŎŐƠỌỎỐỒỔỖỘỚỜỞỠỢÙÚÛÜŨŪŬŮŰŲƯỤỦỨỪỬỮỰÝŸỲỴỶỸ][a-zàáâãäåāăąạảấầẩẫậắằẳẵặæçćĉċčđðèéêëēĕėęěẹẻẽếềểễệìíîïĩīįiỉịñòóôõöøōŏőơọỏốồổỗộớờởỡợùúûüũūŭůűųưụủứừửữựýÿỳỵỷỹ']*)(\s([A-ZÀÁÂÃÄÅĀĂĄẠẢẤẦẨẪẬẮẰẲẴẶÆÇĆĈĊČĐÐÈÉÊËĒĔĖĘĚẸẺẼẾỀỂỄỆÌÍÎÏĨĪĮİỈỊÑÒÓÔÕÖØŌŎŐƠỌỎỐỒỔỖỘỚỜỞỠỢÙÚÛÜŨŪŬŮŰŲƯỤỦỨỪỬỮỰÝŸỲỴỶỸ][a-zàáâãäåāăąạảấầẩẫậắằẳẵặæçćĉċčđðèéêëēĕėęěẹẻẽếềểễệìíîïĩīįiỉịñòóôõöøōŏőơọỏốồổỗộớờởỡợùúûüũūŭůűųưụủứừửữựýÿỳỵỷỹ']*))*$/u;
  if (!regex.test(name)) {
    showAlertModal("Định Dạng Không Hợp Lệ", "Họ và tên phải viết hoa chữ cái đầu mỗi từ và không chứa ký tự đặc biệt", "fas fa-exclamation-triangle");
    return false;
  }
  return true;
}

// Kiểm tra số điện thoại: bắt đầu bằng 0, 10 chữ số, chỉ số
function validatePhoneNumber(phone) {
  phone = phone.trim();
  if (phone.length === 0) {
    showAlertModal("Lỗi Nhập Liệu", "Vui lòng nhập số điện thoại", "fas fa-exclamation-triangle");
    return false;
  }
  if (!/^0\d{9}$/.test(phone)) {
    showAlertModal("Số Điện Thoại Không Hợp Lệ", "Số điện thoại phải bắt đầu bằng số 0 và có đúng 10 chữ số", "fas fa-exclamation-triangle");
    return false;
  }
  return true;
}

// Load địa chỉ từ database
async function loadAddressesFromDatabase() {
    try {
        console.log('Đang load địa chỉ từ database...');
        
        // Hiển thị loading state
        showLoadingState();
        
        const response = await fetch('/KhachHang/Pay/GetAddressesForPayment', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        if (!response.ok) {
            console.error('Lỗi HTTP:', response.status, response.statusText);
            showEmptyState();
            return;
        }

        const result = await response.json();
        console.log('Kết quả load địa chỉ:', result);
        
        if (result.success) {
            // Xóa tất cả địa chỉ hiện có trước khi load lại
            clearExistingAddresses();
            
            if (result.addresses && result.addresses.length > 0) {
                console.log(`Tìm thấy ${result.addresses.length} địa chỉ`);
                
                // Ẩn loading và empty state
                hideLoadingState();
                hideEmptyState();

                // Hiển thị các địa chỉ
                result.addresses.forEach((address, index) => {
                    console.log(`Đang thêm địa chỉ ${index + 1}:`, address);
                    addAddressCard(address);
                });

                // Tự động chọn địa chỉ mặc định
                const defaultAddress = addressContainer.querySelector('input[name="address"]:checked') || 
                                     addressContainer.querySelector('input[name="address"]');
                if (defaultAddress && !defaultAddress.checked) {
                    defaultAddress.checked = true;
                }
                
                console.log('Load địa chỉ thành công');
            } else {
                console.log('Không có địa chỉ nào được tìm thấy');
                hideLoadingState();
                showEmptyState();
            }
        } else {
            console.error('Lỗi từ server:', result.message);
            hideLoadingState();
            showEmptyState();
        }
    } catch (error) {
        console.error('Lỗi khi load địa chỉ:', error);
        hideLoadingState();
        showEmptyState();
    }
}

// Hàm xóa các địa chỉ hiện có
function clearExistingAddresses() {
    const existingAddresses = addressContainer.querySelectorAll('.address-card');
    existingAddresses.forEach(card => card.remove());
}

// Hàm hiển thị loading state
function showLoadingState() {
    const loadingElement = document.getElementById('loadingAddresses');
    const emptyElement = document.getElementById('emptyAddresses');
    if (loadingElement) {
        loadingElement.classList.remove('d-none');
        loadingElement.style.display = 'block';
    }
    if (emptyElement) {
        emptyElement.classList.add('d-none');
        emptyElement.style.display = 'none';
    }
}

// Hàm ẩn loading state
function hideLoadingState() {
    const loadingElement = document.getElementById('loadingAddresses');
    if (loadingElement) {
        loadingElement.classList.add('d-none');
        loadingElement.style.display = 'none';
    }
}

// Hàm hiển thị empty state
function showEmptyState() {
    const emptyElement = document.getElementById('emptyAddresses');
    if (emptyElement) {
        emptyElement.classList.remove('d-none');
        emptyElement.style.display = 'block';
    }
}

// Hàm ẩn empty state
function hideEmptyState() {
    const emptyElement = document.getElementById('emptyAddresses');
    if (emptyElement) {
        emptyElement.classList.add('d-none');
        emptyElement.style.display = 'none';
    }
}

// Thêm address card vào DOM
function addAddressCard(address) {
    const wrapper = document.createElement('div');
    wrapper.className = 'address-card p-4 rounded-3 mb-3 animate-fade-in position-relative';
    wrapper.dataset.addressId = address.id;
    
    wrapper.innerHTML = `
        <div class="d-flex justify-content-between align-items-start">
            <label class="d-flex align-items-start flex-grow-1 cursor-pointer">
                <input type="radio" name="address" value="${address.id}" class="form-check-input me-3 mt-1" style="transform: scale(1.2);" ${address.macDinh ? 'checked' : ''}>
                <div class="flex-grow-1">
                    <div class="address-details">
                        <div class="address-info">
                            <i class="fas fa-user-circle"></i>  
                            <span class="fw-bold">${address.hoTen || 'Không có tên'}</span>
                        </div>
                        <div class="address-info">
                            <i class="fas fa-phone"></i>
                            <span>${address.soDienThoai || 'Không có SĐT'}</span>
                        </div>
                        <div class="address-info">
                            <i class="fas fa-map-marker-alt"></i>
                            <span>${address.diaChiChiTiet || 'Không có địa chỉ'}</span>
                        </div>
                    </div>
                </div>
            </label>
            <button type="button" class="delete-btn delete-address-btn" data-address-id="${address.id}">
                <i class="fas fa-trash"></i>
            </button>
        </div>
    `;
    
    addressContainer.appendChild(wrapper);
    setupAddressCardEvents();
}

saveAddressBtn?.addEventListener('click', async () => {
  const name = document.getElementById('inputName').value.trim();
  const phone = document.getElementById('inputPhone').value.trim();
  const address = document.getElementById('inputAddress').value.trim();
  const city = document.getElementById('inputCity').value.trim();
  const ward = document.getElementById('inputWard').value.trim();

  if (!name || !phone || !address || !city || !ward) {
    showAlertModal("Thông Tin Chưa Đầy Đủ", "Vui lòng điền đầy đủ tất cả các trường bắt buộc để tiếp tục", "fas fa-exclamation-triangle");
    return;
  }

  if (!validateFullName(name)) return;
  if (!validatePhoneNumber(phone)) return;

  try {
    console.log('Đang lưu địa chỉ mới...');
    
    const requestData = {
      hoTen: name,
      soDienThoai: phone,
      diaChiChiTiet: address,
      tinhThanhPho: city,
      phuongXa: ward
    };
    
    console.log('Dữ liệu gửi lên server:', requestData);
    
    // Hiển thị loading state
    saveAddressBtn.disabled = true;
    saveAddressBtn.innerHTML = '<i class="fas fa-spinner fa-spin" style="margin-right: 6px;"></i> Đang lưu...';
    
    // Gửi request lên server để lưu địa chỉ
    const response = await fetch('/KhachHang/Pay/AddAddressFromPayment', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'X-Requested-With': 'XMLHttpRequest'
      },
      body: JSON.stringify(requestData)
    });

    console.log('Response status:', response.status);
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    const result = await response.json();
    console.log('Response data:', result);
    
    if (result.success) {
      console.log('Địa chỉ đã được lưu thành công:', result.address);
      
      // Ẩn phần empty state khi có địa chỉ đầu tiên
      hideEmptyState();

      // Thêm địa chỉ mới vào DOM
      addAddressCard(result.address);

      // Tự động chọn địa chỉ vừa thêm
      const newRadio = addressContainer.querySelector(`input[value="${result.address.id}"]`);
      if (newRadio) {
        newRadio.checked = true;
        console.log('Đã chọn địa chỉ mới làm mặc định');
      }

      // Đóng modal và reset form
      addressModal.classList.remove('d-flex');
      addressModal.classList.add('d-none');
      
      // Reset form
      document.getElementById('addressForm').reset();
      
      // Refresh danh sách địa chỉ để đảm bảo đồng bộ với database
      setTimeout(() => {
        loadAddressesFromDatabase();
      }, 500);
      
      showAlertModal("Thành Công", "Địa chỉ đã được thêm thành công!", "fas fa-check-circle");
    } else {
      console.error('Lỗi từ server:', result.message);
      showAlertModal("Lỗi", result.message || "Có lỗi xảy ra khi thêm địa chỉ", "fas fa-exclamation-triangle");
    }
  } catch (error) {
    console.error('Lỗi khi lưu địa chỉ:', error);
    showAlertModal("Lỗi Kết Nối", "Không thể kết nối tới server. Vui lòng thử lại.", "fas fa-exclamation-triangle");
  } finally {
    // Reset button state
    saveAddressBtn.disabled = false;
    saveAddressBtn.innerHTML = '<i class="fas fa-save" style="margin-right: 6px;"></i> LƯU ĐỊA CHỈ';
  }
});

function setupAddressCardEvents() {
  const addressCards = document.querySelectorAll('.address-card');
  addressCards.forEach(card => {
    const radio = card.querySelector('input[type="radio"]');
    const deleteBtn = card.querySelector('.delete-address-btn');

    // Xóa event cũ để tránh duplicate
    card.onclick = null;
    if (deleteBtn) deleteBtn.onclick = null;

    card.addEventListener('click', () => {
      addressCards.forEach(c => {
        c.classList.remove('selected');
        c.querySelector('input[type="radio"]').checked = false;
      });
      card.classList.add('selected');
      radio.checked = true;
      
      // Lưu địa chỉ được chọn vào localStorage (optional)
      localStorage.setItem('selectedAddressId', radio.value);
      console.log('Đã chọn địa chỉ:', radio.value);
    });

    if (deleteBtn) {
      deleteBtn.addEventListener('click', async (e) => {
        e.stopPropagation();
        
        console.log('=== XÓA ĐỊA CHỈ - KHÔNG CÓ CONFIRM ===', new Date().toISOString());
        
        const addressId = deleteBtn.dataset.addressId;
        
        try {
          console.log('Đang xóa địa chỉ:', addressId);
          
          const response = await fetch('/KhachHang/DiaChi/DeleteAddress', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              'X-Requested-With': 'XMLHttpRequest'
            },
            body: JSON.stringify({ id: parseInt(addressId) })
          });

          if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
          }

          const result = await response.json();
          
          if (result.success) {
            // Xóa địa chỉ được chọn khỏi localStorage nếu trùng
            const selectedAddressId = localStorage.getItem('selectedAddressId');
            if (selectedAddressId === addressId) {
              localStorage.removeItem('selectedAddressId');
            }
            
            // Reload danh sách địa chỉ để đảm bảo đồng bộ với database
            await loadAddressesFromDatabase();
            
            // Không hiển thị thông báo
            console.log('Đã xóa địa chỉ thành công');
          } else {
            console.error('Lỗi xóa địa chỉ từ server:', result.message);
            // Không hiển thị thông báo lỗi, chỉ log
            console.log('Không thể xóa địa chỉ:', result.message);
          }
        } catch (error) {
          console.error('Lỗi khi xóa địa chỉ:', error);
          // Không hiển thị thông báo lỗi, chỉ log
          console.log('Lỗi kết nối khi xóa địa chỉ');
        }
      });
    }
  });
  
  // Khôi phục địa chỉ được chọn từ localStorage (nếu có)
  const selectedAddressId = localStorage.getItem('selectedAddressId');
  if (selectedAddressId) {
    const targetRadio = document.querySelector(`input[name="address"][value="${selectedAddressId}"]`);
    if (targetRadio) {
      targetRadio.checked = true;
      targetRadio.closest('.address-card').classList.add('selected');
      console.log('Đã khôi phục địa chỉ được chọn:', selectedAddressId);
    }
  }
}

// Payment method selection
document.querySelectorAll('.payment-method').forEach(method => {
  method.addEventListener('click', () => {
    document.querySelectorAll('.payment-method').forEach(m => m.classList.remove('selected'));
    method.classList.add('selected');
    method.querySelector('input[type="radio"]').checked = true;
  });
});

// Xử lý nút quay lại giỏ hàng
document.getElementById('backToCartBtn').addEventListener('click', () => {
  // Kiểm tra nếu là chế độ "mua ngay"
  if (window.isBuyNow) {
    history.back(); // Quay lại trang chi tiết sản phẩm
  } else {
    window.location.href = '/KhachHang/Cart'; // Quay lại giỏ hàng
  }
});

// Xử lý hoàn tất đơn hàng
document.getElementById('completeOrderBtn').addEventListener('click', () => {
  const orderItems = getValidOrderItems(); // Sử dụng hàm getValidOrderItems để hỗ trợ cả chế độ mua ngay và giỏ hàng
  
  if (orderItems.length === 0) {
    const errorMessage = window.isBuyNow ? "Không thể xử lý sản phẩm mua ngay. Vui lòng thử lại!" : "Không có sản phẩm nào để thanh toán. Vui lòng thêm sản phẩm vào giỏ hàng!";
    showAlertModal("Giỏ Hàng Trống", errorMessage, "fas fa-exclamation-triangle");
    return;
  }

  // Bỏ qua việc kiểm tra địa chỉ - đặt hàng trực tiếp với COD
  // Kiểm tra phương thức thanh toán (đã được đặt mặc định là COD)
  const selectedPayment = document.querySelector('input[name="payment"]:checked');
  if (!selectedPayment) {
    // Tự động chọn COD nếu chưa có phương thức nào được chọn
    const codPayment = document.querySelector('input[name="payment"]');
    if (codPayment) {
      codPayment.checked = true;
    }
  }

  // Xử lý đơn hàng ngay lập tức với thông tin mặc định
  processOrderFromJS();
});

// Hàm xử lý đơn hàng
async function processOrderFromJS() {
  try {
    // Lấy thông tin cần thiết - bỏ qua việc kiểm tra địa chỉ
    const selectedPayment = document.querySelector('input[name="payment"]:checked');
    const orderItems = getValidOrderItems(); // Sử dụng hàm getValidOrderItems thay vì lấy trực tiếp từ localStorage
    
    console.log('ProcessOrder - isBuyNow:', window.isBuyNow);
    console.log('ProcessOrder - orderItems:', orderItems);
    
    // Đảm bảo có phương thức thanh toán (mặc định COD)
    if (!selectedPayment) {
      const codPayment = document.querySelector('input[name="payment"]');
      if (codPayment) {
        codPayment.checked = true;
      } else {
        showAlertModal("Lỗi", "Không thể xác định phương thức thanh toán", "error");
        return;
      }
    }
    
    if (orderItems.length === 0) {
      const errorMessage = window.isBuyNow ? "Không thể xử lý sản phẩm mua ngay" : "Không có sản phẩm nào để đặt hàng";
      showAlertModal("Lỗi", errorMessage, "error");
      return;
    }

    // Hiển thị thông báo đang xử lý
    showAlertModal("Đang Xử Lý", "Đơn hàng của bạn đang được xử lý...", "fas fa-spinner", true);

    // Chuẩn bị dữ liệu đơn hàng - mặc định sử dụng COD
    const paymentMethod = "COD";
    
    // Tính tổng tiền
    const subtotal = orderItems.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    const shippingFee = 30000;
    const totalAmount = subtotal + shippingFee;

    // Chuyển đổi orderItems thành format phù hợp cho server
    const orderItemsForServer = orderItems.map(item => ({
      ProductId: item.id || item.productId, // Sử dụng product ID
      SoLuong: item.quantity,
      DonGia: item.price
    }));

    console.log('Order items for server:', orderItemsForServer);

    // Gửi request đến server với thông tin mặc định (không cần địa chỉ)
    const response = await fetch('/KhachHang/Pay/ProcessOrder', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'X-Requested-With': 'XMLHttpRequest'
      },
      body: JSON.stringify({
        paymentMethod: paymentMethod,
        totalAmount: totalAmount,
        orderItems: orderItemsForServer
      })
    });

    const result = await response.json();
    
    if (result.success) {
      // Chỉ xóa localStorage khi không phải chế độ "mua ngay"
      if (!window.isBuyNow) {
        localStorage.removeItem('orderItems');
        localStorage.removeItem('cartSelected');
        localStorage.removeItem('selectedCartItems');
        console.log('Đã xóa dữ liệu localStorage (chế độ giỏ hàng)');
      } else {
        console.log('Bỏ qua xóa localStorage (chế độ mua ngay)');
      }
      
      // Cập nhật số lượng giỏ hàng trên header (chỉ khi không phải mua ngay)
      if (!window.isBuyNow) {
        await updateCartCountAfterPayment();
      }
      
      // Đóng modal đang xử lý
      closeAlertModal();
      
      // Hiện modal thành công
      const successModal = document.getElementById('paymentSuccessModal');
      successModal.classList.remove('d-none');
      successModal.classList.add('d-flex');
      
      // Gắn sự kiện đóng modal
      setTimeout(() => {
        const closeBtn = document.getElementById('closeSuccessModalBtn');
        if (closeBtn) {
          closeBtn.onclick = closeSuccessModal;
          closeBtn.addEventListener('click', closeSuccessModal);
        }
      }, 200);
    } else {
      closeAlertModal();
      showAlertModal("Lỗi", result.message || "Có lỗi xảy ra khi đặt hàng", "error");
    }
  } catch (error) {
    console.error('Lỗi khi xử lý đơn hàng:', error);
    closeAlertModal();
    showAlertModal("Lỗi", "Có lỗi xảy ra khi đặt hàng. Vui lòng thử lại.", "error");
  }
}

// Xử lý đóng modal thành công (backup - gắn sự kiện từ đầu)
document.addEventListener('DOMContentLoaded', function() {
  console.log('DOM đã load, backup event cho nút X...');
  
  // Backup method - gắn sự kiện toàn cục
  document.addEventListener('click', function(e) {
    if (e.target && (e.target.id === 'closeSuccessModalBtn' || e.target.closest('#closeSuccessModalBtn'))) {
      console.log('Global click: Nút X được click!');
      e.preventDefault();
      e.stopPropagation();
      closeSuccessModal();
    }
    
    // Xử lý nút "Xác nhận hủy" bằng event delegation
    if (e.target && e.target.id === 'alertModalOk' && e.target.innerHTML.includes('Xác nhận hủy')) {
      console.log('Global click: Nút Xác nhận hủy được click!');
      e.preventDefault();
      e.stopPropagation();
      
      // Đóng alert modal
      const alertModal = document.getElementById('alertModal');
      if (alertModal) {
        alertModal.classList.remove('d-flex');
        alertModal.classList.add('d-none');
      }
      
      // Đóng modal MoMo
      const momoQrModal = document.getElementById('momoQrModal');
      if (momoQrModal) {
        momoQrModal.classList.remove('d-flex');
        momoQrModal.classList.add('d-none');
        console.log('Modal MoMo đã được đóng bằng event delegation');
      }
      
      // Reset nút OK
      setTimeout(() => {
        if (e.target) {
          e.target.innerHTML = '<i class="fas fa-check"></i> Đã hiểu';
          e.target.onclick = closeAlertModal;
        }
      }, 100);
    }
  });
  
  // Thêm sự kiện click vào overlay để đóng modal
  const successModal = document.getElementById('paymentSuccessModal');
  if (successModal) {
    successModal.addEventListener('click', function(e) {
      // Chỉ đóng khi click vào overlay (không phải modal-content)
      if (e.target === successModal) {
        console.log('Click vào overlay, đóng modal...');
        closeSuccessModal();
      }
    });
  }
});

// Xử lý đóng modal không có địa chỉ
const closeNoAddressModalBtn = document.getElementById('closeNoAddressModalBtn');
const closeNoAddressBtn = document.getElementById('closeNoAddressBtn');
const addAddressFromModalBtn = document.getElementById('addAddressFromModalBtn');
const noAddressModal = document.getElementById('noAddressModal');

[closeNoAddressModalBtn, closeNoAddressBtn].forEach(btn => {
  btn?.addEventListener('click', () => {
    noAddressModal.classList.remove('d-flex');
    noAddressModal.classList.add('d-none');
  });
});

// Xử lý nút thêm địa chỉ từ modal thông báo
addAddressFromModalBtn?.addEventListener('click', () => {
  // Đóng modal thông báo
  noAddressModal.classList.remove('d-flex');
  noAddressModal.classList.add('d-none');
  
  // Mở modal thêm địa chỉ
  addressModal.classList.remove('d-none');
  addressModal.classList.add('d-flex');
});

// Thêm hiệu ứng hover cho nút quay lại
const backBtn = document.getElementById('backToCartBtn');
backBtn.addEventListener('mouseenter', () => {
  backBtn.style.background = 'var(--lighter)';
  backBtn.style.borderColor = 'var(--primary)';
  backBtn.style.transform = 'translateY(-2px)';
});

backBtn.addEventListener('mouseleave', () => {
  backBtn.style.background = 'var(--white)';
  backBtn.style.borderColor = 'var(--border)';
  backBtn.style.transform = 'translateY(0)';
});

function formatVND(n) {
  return n.toLocaleString('vi-VN') + '₫';
}

// Kiểm tra dữ liệu từ localStorage và xác thực
function getValidOrderItems() {
  try {
    console.log('=== getValidOrderItems called ===');
    console.log('window.isBuyNow:', window.isBuyNow);
    console.log('window.buyNowData:', window.buyNowData);
    
    // Kiểm tra nếu là chế độ "mua ngay"
    if (window.isBuyNow && window.buyNowData) {
      console.log('Processing Buy Now item:', window.buyNowData);
      // Chuyển đổi dữ liệu "mua ngay" sang format phù hợp
      const buyNowItem = {
        id: window.buyNowData.productId, // Sử dụng productId từ server
        productId: window.buyNowData.productId,
        name: window.buyNowData.name,
        price: window.buyNowData.price,
        quantity: window.buyNowData.quantity,
        total: window.buyNowData.total,
        linkAnh: window.buyNowData.linkAnh || '/images/noimage.jpg'
      };
      
      console.log('Processed buyNowItem:', buyNowItem);
      
      // Validate dữ liệu
      if (buyNowItem.name && 
          buyNowItem.price > 0 && 
          buyNowItem.quantity > 0 &&
          buyNowItem.productId > 0) {
        console.log('Valid Buy Now item:', buyNowItem);
        return [buyNowItem];
      } else {
        console.error('Invalid Buy Now data:', buyNowItem);
        console.error('Failed validation checks:');
        console.error('- name:', buyNowItem.name);
        console.error('- price > 0:', buyNowItem.price > 0);
        console.error('- quantity > 0:', buyNowItem.quantity > 0);
        console.error('- productId > 0:', buyNowItem.productId > 0);
        return [];
      }
    }
    
    // Nếu không phải "mua ngay", lấy từ localStorage như bình thường
    const orderItems = JSON.parse(localStorage.getItem('orderItems')) || [];
    
    // Kiểm tra xem localStorage có dữ liệu hợp lệ không
    if (!Array.isArray(orderItems) || orderItems.length === 0) {
      return [];
    }
    
    // Validate từng item
    const validItems = orderItems.filter(item => {
      return item && 
             typeof item.name === 'string' && 
             typeof item.price === 'number' && 
             typeof item.quantity === 'number' &&
             typeof item.idBienThe === 'number' &&
             item.price > 0 && 
             item.quantity > 0 &&
             item.idBienThe > 0;
    });
    
    // Đảm bảo mỗi item có linkAnh (nếu không có thì dùng default)
    const processedItems = validItems.map(item => ({
      ...item,
      linkAnh: item.linkAnh || '/images/noimage.jpg'
    }));
    
    return processedItems;
  } catch (error) {
    console.error('Error parsing orderItems:', error);
    return [];
  }
}

// Hàm khởi tạo hiển thị sản phẩm
function initializeOrderDisplay() {
  console.log('=== initializeOrderDisplay called ===');
  const container = document.getElementById('orderItemsContainer');
  if (!container) {
    console.error('orderItemsContainer not found!');
    return;
  }
  
  const orderItems = getValidOrderItems();
  console.log('orderItems from getValidOrderItems:', orderItems);
  console.log('orderItems.length:', orderItems.length);

  if (orderItems.length === 0) {
    console.log('No order items found, showing empty state');
    const isBuyNowMode = window.isBuyNow || false;
    const backButtonText = isBuyNowMode ? 'Quay lại sản phẩm' : 'Quay lại giỏ hàng';
    const backButtonLink = isBuyNowMode ? 'javascript:history.back()' : '/KhachHang/Cart';
    
    container.innerHTML = `
      <div class="text-center py-5">
        <i class="fas fa-shopping-cart mb-3" style="font-size: 3rem; color: #2E7D32; opacity: 0.5;"></i>
        <p class="text-muted fs-5">Không có sản phẩm nào trong đơn hàng.</p>
        <p class="text-muted">${isBuyNowMode ? 'Vui lòng thử lại.' : 'Vui lòng quay lại giỏ hàng để chọn sản phẩm.'}</p>
        <button onclick="window.location.href='${backButtonLink}'" class="btn btn-primary mt-2">
          <i class="fas fa-arrow-left me-2"></i>${backButtonText}
        </button>
      </div>`;
      
    // Ẩn nút hoàn tất đơn hàng khi không có sản phẩm
    const completeBtn = document.getElementById('completeOrderBtn');
    if (completeBtn) {
      completeBtn.style.display = 'none';
    }
    
    // Cập nhật tổng tiền về 0
    const subtotalElement = document.getElementById('subtotalAmount');
    const totalElement = document.getElementById('totalAmount');
    if (subtotalElement) subtotalElement.textContent = formatVND(0);
    if (totalElement) totalElement.textContent = formatVND(30000); // Chỉ phí vận chuyển
  } else {
    console.log('Found order items, displaying products...');
    let subtotal = 0;
    container.innerHTML = orderItems.map(item => {
      console.log('Processing item:', item);
      const itemTotal = item.price * item.quantity;
      subtotal += itemTotal;
    return `
      <div class="product-item d-flex align-items-center p-3 mb-2 rounded-3">
        <div class="product-image-wrapper me-3">
          <img src="${item.linkAnh || '/images/noimage.jpg'}" alt="Product" class="img-fluid" />
        </div>
        <div class="flex-grow-1">
          <h6 class="fw-semibold mb-1" style="color: var(--primary); font-size: 1rem;">${item.name}</h6>
          <div class="d-flex justify-content-between align-items-center">
            <div class="d-flex align-items-center text-muted">
              <i class="fas fa-cube me-1" style="color: var(--primary); font-size: 0.8rem;"></i>
              <small>${item.quantity} x ${formatVND(item.price)}</small>
            </div>
            <div class="fw-bold text-end" style="color: var(--primary); font-size: 1.1rem;">${formatVND(itemTotal)}</div>
          </div>
        </div>
      </div>`;
    }).join('');

    // Cập nhật tổng tiền
    const subtotalElement = document.getElementById('subtotalAmount');
    const totalElement = document.getElementById('totalAmount');
    if (subtotalElement) subtotalElement.textContent = formatVND(subtotal);
    if (totalElement) totalElement.textContent = formatVND(subtotal + 30000);
    
    // Hiển thị nút hoàn tất đơn hàng
    const completeBtn = document.getElementById('completeOrderBtn');
    if (completeBtn) {
      completeBtn.style.display = 'block';
    }
  }
}

document.getElementById("backToCartBtn")?.addEventListener("click", function () {
    // Kiểm tra nếu là chế độ "mua ngay"
    if (window.isBuyNow) {
      history.back(); // Quay lại trang chi tiết sản phẩm
    } else {
      window.location.href = "/KhachHang/Cart"; // Quay lại giỏ hàng
    }
});

// Alert Modal System - Single unified implementation
function showAlertModal(title, message, icon = 'fas fa-exclamation-circle', autoClose = false) {
    document.getElementById('alertModalTitle').textContent = title;
    document.getElementById('alertModalMessage').textContent = message;
    document.getElementById('alertModalIcon').className = icon;

    const footer = document.querySelector('#alertModal .modal-footer');
    const closeBtn = document.getElementById('alertModalClose');

    if (autoClose) {
        footer.style.display = 'none';      // Ẩn footer
        closeBtn.style.display = 'none';    // Ẩn nút X
        // Đặt biến trạng thái để kiểm tra đã đóng chưa
        const alertModal = document.getElementById('alertModal');
        alertModal._autoCloseTimeout = setTimeout(() => {
            // Chỉ đóng nếu modal vẫn đang mở
            if (alertModal.classList.contains('d-flex')) {
                closeAlertModal();
            }
        }, 2000);
    } else {
        footer.style.display = 'flex';      // Hiện footer
        closeBtn.style.display = '';        // Hiện nút X
    }

    document.getElementById('alertModal').classList.remove('d-none');
    document.getElementById('alertModal').classList.add('d-flex');
    
    // Thêm sự kiện click overlay để đóng modal
    const alertModal = document.getElementById('alertModal');
    function outsideClickHandler(e) {
        // Chỉ đóng khi click vào chính alertModal (overlay), không phải modal-content
        if (e.target === alertModal) {
            closeAlertModal();
        }
    }
    alertModal.addEventListener('mousedown', outsideClickHandler);
    alertModal._outsideClickHandler = outsideClickHandler;
}

function closeAlertModal() {
    const alertModal = document.getElementById('alertModal');
    alertModal.classList.remove('d-flex');
    alertModal.classList.add('d-none');
    
    // Chỉ reset nút OK nếu không phải đang trong quá trình xử lý đặc biệt
    const alertModalOk = document.getElementById('alertModalOk');
    const isProcessingSpecialAction = alertModalOk && alertModalOk.innerHTML.includes('Xác nhận hủy');
    
    if (!isProcessingSpecialAction) {
        // Reset nút OK về trạng thái ban đầu (chỉ khi không đang xử lý action đặc biệt)
        alertModalOk.innerHTML = '<i class="fas fa-check"></i> Đã hiểu';
        alertModalOk.onclick = closeAlertModal;
    }
    
    // Xóa sự kiện click overlay khi đóng
    if (alertModal._outsideClickHandler) {
        alertModal.removeEventListener('mousedown', alertModal._outsideClickHandler);
        delete alertModal._outsideClickHandler;
    }
    // Nếu có timeout autoClose thì clear luôn
    if (alertModal._autoCloseTimeout) {
        clearTimeout(alertModal._autoCloseTimeout);
        delete alertModal._autoCloseTimeout;
    }
}

// Event listeners for alert modal
document.getElementById('alertModalClose').addEventListener('click', closeAlertModal);
document.getElementById('alertModalOk').addEventListener('click', closeAlertModal);

// Xử lý modal MoMo QR Code
document.addEventListener('DOMContentLoaded', function() {
  // Xử lý đóng modal MoMo QR
  const cancelMomoPaymentBtn = document.getElementById('cancelMomoPaymentBtn');
  const confirmMomoPaymentBtn = document.getElementById('confirmMomoPaymentBtn');
  const momoQrModal = document.getElementById('momoQrModal');

  // Hàm để đóng modal MoMo
  function closeMomoModal() {
    if (momoQrModal) {
      momoQrModal.classList.remove('d-flex');
      momoQrModal.classList.add('d-none');
      console.log('Modal MoMo đã được đóng');
    }
  }

  // Hàm xử lý xác nhận hủy thanh toán MoMo
  function confirmCancelMomoPayment() {
    console.log('Đang thực hiện hủy thanh toán MoMo...');
    
    // Đóng alert modal trước
    closeAlertModal();
    
    // Đóng modal MoMo với một chút delay để mượt mà
    setTimeout(() => {
      closeMomoModal();
      console.log('Đã hủy thanh toán MoMo và quay về trang thanh toán');
    }, 200);
  }

  // Đóng modal khi click nút Hủy
  cancelMomoPaymentBtn?.addEventListener('click', () => {
    // Hiện modal xác nhận hủy
    showAlertModal(
      "Xác nhận hủy thanh toán", 
      "Bạn có chắc chắn muốn hủy thanh toán MoMo và quay lại trang thanh toán?", 
      "fas fa-question-circle"
    );
    
    // Thay đổi nút OK thành nút xác nhận hủy và thêm logic đóng modal MoMo
    setTimeout(() => {
      const alertModalOk = document.getElementById('alertModalOk');
      if (alertModalOk) {
        // Lưu trạng thái ban đầu
        const originalText = alertModalOk.innerHTML;
        const originalOnClick = alertModalOk.onclick;
        
        // Thay đổi nút thành "Xác nhận hủy"
        alertModalOk.innerHTML = '<i class="fas fa-check"></i> Xác nhận hủy';
        
        // Xóa event listener cũ và thêm mới
        alertModalOk.onclick = null;
        alertModalOk.removeEventListener('click', closeAlertModal);
        
        // Thêm event listener mới cho "Xác nhận hủy"
        alertModalOk.onclick = confirmCancelMomoPayment;
        
        // Thêm fallback event listener
        alertModalOk.addEventListener('click', function(e) {
          e.preventDefault();
          e.stopPropagation();
          confirmCancelMomoPayment();
        });
        
        // Reset nút về trạng thái ban đầu sau 1 phút (cleanup)
        setTimeout(() => {
          if (alertModalOk) {
            alertModalOk.innerHTML = originalText || '<i class="fas fa-check"></i> Đã hiểu';
            alertModalOk.onclick = originalOnClick || closeAlertModal;
          }
        }, 60000);
      }
    }, 100);
  });

  // Xác nhận thanh toán MoMo
  confirmMomoPaymentBtn?.addEventListener('click', () => {
    // Đóng modal MoMo QR
    closeMomoModal();
    
    // Xử lý đơn hàng
    processOrder();
  });
});

