
// Lấy ID sản phẩm từ URL
function getProductIdFromURL() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('id') || '1';
}

// Load thông tin sản phẩm
function loadProductInfo() {
    const productId = getProductIdFromURL();
    
    // TODO: Replace with actual API call
    // const product = await fetchProductById(productId);
    const product = productData[productId];
    
    if (product) {
        // Update DOM elements with product data
        document.getElementById('productTitle').textContent = product.title;
        document.getElementById('breadcrumbProduct').textContent = product.title;
        document.getElementById('productRating').textContent = product.rating;
        document.getElementById('productRatingCount').textContent = product.ratingCount;
        document.getElementById('productCurrentPrice').textContent = product.currentPrice;
        document.getElementById('productCategory').textContent = product.categoryName;
        document.getElementById('productBrand').textContent = product.brand;
        document.getElementById('productExpiry').textContent = product.expiry;
        document.getElementById('productOrigin').textContent = product.origin;
        document.getElementById('productIngredients').textContent = product.ingredients;
        document.getElementById('productUsage').textContent = product.usage;
        document.getElementById('productDescription').textContent = product.description;
        document.getElementById('mainImage').src = product.image;
        
        // Load product options
        loadProductOptions(product.options);
        
        // Load suggested products
        loadSuggestedProducts(product.category, productId);
        
        // Update page title
        document.title = product.title + ' - LittleFish Beauty Mỹ Phẩm Thiên Nhiên';
    } else {
        // TODO: Handle product not found case
        console.log('Product not found');
        // Redirect to 404 page or show error message
    }
}

// Load tùy chọn sản phẩm
function loadProductOptions(options) {
    const optionsContainer = document.getElementById('productOptions');
    optionsContainer.innerHTML = '';
    
    if (!options) return;
    
    Object.keys(options).forEach(optionName => {
        const optionGroup = document.createElement('div');
        optionGroup.className = 'option-group';
        
        const optionLabel = document.createElement('label');
        optionLabel.className = 'option-label';
        optionLabel.textContent = optionName + ':';
        
        const optionItems = document.createElement('div');
        optionItems.className = 'option-items';
        
        options[optionName].forEach((value, index) => {
            const optionItem = document.createElement('div');
            optionItem.className = 'option-item' + (index === 0 ? ' active' : '');
            optionItem.textContent = value;
            optionItem.onclick = () => selectOption(optionItem, optionName);
            optionItems.appendChild(optionItem);
        });
        
        optionGroup.appendChild(optionLabel);
        optionGroup.appendChild(optionItems);
        optionsContainer.appendChild(optionGroup);
    });
}

// Chọn tùy chọn sản phẩm
function selectOption(selectedItem, optionName) {
    const optionItems = selectedItem.parentNode.querySelectorAll('.option-item');
    optionItems.forEach(item => item.classList.remove('active'));
    selectedItem.classList.add('active');
    
    // TODO: Update price or product variant based on selection
    console.log(`Selected ${optionName}: ${selectedItem.textContent}`);
}

// Load sản phẩm gợi ý
function loadSuggestedProducts(category, currentProductId) {
    const suggestedContainer = document.getElementById('suggestedProductsGrid');
    suggestedContainer.innerHTML = '';
    
    // TODO: Replace with actual API call
    // const suggestedProducts = await fetchSuggestedProducts(category, currentProductId);
    
    // For now, show placeholder
    console.log(`Loading suggested products for category: ${category}`);
    
    // Placeholder for suggested products
    for (let i = 0; i < 4; i++) {
        const productCard = document.createElement('div');
        productCard.className = 'suggested-product-card';
        productCard.innerHTML = `
            <div style="width: 100%; height: 200px; background: #f0f0f0; display: flex; align-items: center; justify-content: center; color: #666;">
                Sản phẩm gợi ý ${i + 1}
            </div>
            <div class="suggested-product-info">
                <h3 class="suggested-product-title">Tên sản phẩm sẽ được tải từ API</h3>
                <div class="suggested-product-price">0₫</div>
                <div class="suggested-product-rating">
                    <span class="suggested-rating-stars">⭐⭐⭐⭐⭐</span>
                    <span>0</span>
                </div>
            </div>
        `;
        
        productCard.onclick = () => {
            // TODO: Navigate to actual product
            console.log(`Navigate to suggested product ${i + 1}`);
        };
        
        suggestedContainer.appendChild(productCard);
    }
}

// Thay đổi hình ảnh chính
function changeMainImage(src) {
    const mainImage = document.getElementById('mainImage');
    if (mainImage) {
        mainImage.src = src;
    }
    
    // Cập nhật thumbnail active
    document.querySelectorAll('.thumbnail').forEach(thumb => {
        thumb.classList.remove('active');
        if (thumb.src === src) {
            thumb.classList.add('active');
        }
    });
}

// Điều khiển số lượng
function increaseQuantity() {
    const input = document.getElementById('productQuantity');
    if (input) {
        let value = parseInt(input.value);
        if (value < 99) {
            input.value = value + 1;
        }
    }
}

function decreaseQuantity() {
    const input = document.getElementById('productQuantity');
    if (input) {
        let value = parseInt(input.value);
        if (value > 1) {
            input.value = value - 1;
        }
    }
}

// Thêm vào giỏ hàng
function addToCart() {
    const quantityInput = document.getElementById('productQuantity');
    const productTitleElement = document.getElementById('productTitle');
    
    if (quantityInput && productTitleElement) {
        const quantity = quantityInput.value;
        const productTitle = productTitleElement.textContent;
        
        // TODO: Integrate with actual cart API
        // await addProductToCart(productId, quantity, selectedOptions);
        
        console.log(`Adding to cart: ${quantity} x ${productTitle}`);
        alert(`Đã thêm ${quantity} x "${productTitle}" vào giỏ hàng!`);
    }
}

// Mua ngay
function buyNow() {
    const quantityInput = document.getElementById('productQuantity');
    const productTitleElement = document.getElementById('productTitle');
    
    if (quantityInput && productTitleElement) {
        const quantity = quantityInput.value;
        const productTitle = productTitleElement.textContent;
        
        // TODO: Integrate with checkout process
        console.log(`Buy now: ${quantity} x ${productTitle}`);
        alert(`Mua ngay ${quantity} x "${productTitle}". Chuyển đến trang thanh toán...`);
    }
}

// Search functionality
// Quản lý lịch sử tìm kiếm
let searchHistory = JSON.parse(localStorage.getItem('searchHistory')) || [];

// Elements
const searchInput = document.getElementById('searchInput');
const searchBtn = document.getElementById('searchBtn');
const searchSuggestions = document.getElementById('searchSuggestions');
const searchFilters = document.getElementById('searchFilters');
const advancedSearchToggle = document.getElementById('advancedSearchToggle');
const priceSlider = document.getElementById('priceSlider');
const priceValue = document.getElementById('priceValue');

// Update price display when slider changes
if (priceSlider && priceValue) {
    priceSlider.addEventListener('input', function() {
        const value = parseInt(this.value);
        priceValue.textContent = value.toLocaleString('vi-VN') + '₫';
    });
}

// Hiển thị gợi ý tìm kiếm
function showSearchSuggestions(query = '') {
    if (!searchSuggestions) return;
    
    searchSuggestions.innerHTML = '';
    
    // Hiển thị lịch sử nếu không có query
    if (!query.trim()) {
        const historyDiv = document.createElement('div');
        historyDiv.className = 'search-history';
        historyDiv.innerHTML = `
            <div class="search-history-title">
                Tìm kiếm gần đây 
                <span class="clear-history" onclick="clearSearchHistory()">Xóa</span>
            </div>
            <div id="historyItemsInner"></div>
        `;
        searchSuggestions.appendChild(historyDiv);
        
        const historyItemsInner = document.getElementById('historyItemsInner');
        searchHistory.slice(-5).reverse().forEach(term => {
            const historyItem = document.createElement('span');
            historyItem.className = 'history-item';
            historyItem.textContent = term;
            historyItem.onclick = () => {
                if (searchInput) {
                    searchInput.value = term;
                    performSearch(term);
                    hideSearchSuggestions();
                }
            };
            historyItemsInner.appendChild(historyItem);
        });
    } else {
        // TODO: Integrate with real product search API
        // For now, show basic search functionality
        const suggestionItem = document.createElement('div');
        suggestionItem.className = 'suggestion-item';
        suggestionItem.innerHTML = `
            <div><strong>Tìm kiếm: "${query}"</strong></div>
            <div class="suggestion-category">Nhấn Enter để tìm kiếm</div>
        `;
        suggestionItem.onclick = () => {
            performSearch(query);
            hideSearchSuggestions();
        };
        searchSuggestions.appendChild(suggestionItem);
    }
    
    searchSuggestions.style.display = 'block';
}

function hideSearchSuggestions() {
    if (searchSuggestions) {
        searchSuggestions.style.display = 'none';
    }
}

function getCategoryName(category) {
    const categoryNames = {
        'skincare': 'Chăm sóc da',
        'makeup': 'Trang điểm',
        'haircare': 'Dưỡng tóc',
        'suncare': 'Chống nắng',
        'essential-oils': 'Tinh dầu',
        'body-care': 'Chăm sóc cơ thể'
    };
    return categoryNames[category] || category;
}

function formatPrice(price) {
    return new Intl.NumberFormat('vi-VN').format(price) + ' VNĐ';
}

// Thực hiện tìm kiếm
function performSearch(query) {
    if (query.trim()) {
        // Lưu vào lịch sử
        if (!searchHistory.includes(query)) {
            searchHistory.push(query);
            if (searchHistory.length > 10) {
                searchHistory.shift();
            }
            localStorage.setItem('searchHistory', JSON.stringify(searchHistory));
        }
        
        // Redirect to products page with search query
        window.location.href = `sanpham.html?search=${encodeURIComponent(query)}`;
    }
    hideSearchSuggestions();
}

function filterByCategory(category) {
    window.location.href = `sanpham.html?category=${category}`;
}

function clearSearchHistory() {
    searchHistory = [];
    localStorage.removeItem('searchHistory');
    hideSearchSuggestions();
}

// Event listeners
if (searchInput) {
    searchInput.addEventListener('input', (e) => {
        const query = e.target.value;
        showSearchSuggestions(query);
    });

    searchInput.addEventListener('focus', () => {
        showSearchSuggestions(searchInput.value);
    });

    searchInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
            performSearch(searchInput.value);
            hideSearchSuggestions();
        }
    });
}

if (searchBtn) {
    searchBtn.addEventListener('click', () => {
        if (searchInput) {
            performSearch(searchInput.value);
            hideSearchSuggestions();
        }
    });
}

// Toggle advanced search
if (advancedSearchToggle) {
    advancedSearchToggle.addEventListener('click', (e) => {
        e.preventDefault();
        if (searchFilters) {
            const isVisible = searchFilters.style.display === 'block';
            searchFilters.style.display = isVisible ? 'none' : 'block';
            hideSearchSuggestions();
        }
    });
}

// Apply filters
const applyFiltersBtn = document.getElementById('applyFilters');
if (applyFiltersBtn) {
    applyFiltersBtn.addEventListener('click', () => {
        const category = document.getElementById('categoryFilter')?.value;
        const maxPrice = document.getElementById('priceSlider')?.value;
        const sortBy = document.getElementById('sortBy')?.value;
        
        let url = 'sanpham.html?';
        if (category) url += `category=${category}&`;
        if (maxPrice && maxPrice < 500000) url += `maxPrice=${maxPrice}&`;
        if (sortBy && sortBy !== 'relevance') url += `sort=${sortBy}&`;
        
        window.location.href = url;
    });
}

// Dropdown menu trong navbar - chuyển trang ngay lập tức
document.querySelectorAll('.navbar .dropdown-item[data-category]').forEach(item => {
    item.addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        const category = e.target.getAttribute('data-category');
        // Chuyển trang ngay lập tức
        window.location.href = `sanpham.html?category=${category}`;
    });
});

// Close dropdowns when clicking outside
document.addEventListener('click', (e) => {
    if (!e.target.closest('.search-container')) {
        hideSearchSuggestions();
        if (searchFilters) {
            searchFilters.style.display = 'none';
        }
    }
});

// Ngăn không cho suggestions bị ẩn khi click vào chính nó
if (searchSuggestions) {
    searchSuggestions.addEventListener('click', (e) => {
        e.stopPropagation();
    });
}

// Bootstrap dropdown support for categories
const categoryDropdown = document.getElementById('categoryDropdown');
if (categoryDropdown) {
    categoryDropdown.addEventListener('show.bs.dropdown', function() {
        hideSearchSuggestions();
        if (searchFilters) {
            searchFilters.style.display = 'none';
        }
    });
}

// Dữ liệu đánh giá mẫu
const reviewsData = [
    {
        id: 1,
        userName: 'Minh Anh',
        avatar: 'https://images.unsplash.com/photo-1494790108755-2616b612b786?ixlib=rb-4.0.3&auto=format&fit=crop&w=100&q=80',
        rating: 5,
        date: '2025-08-05',
        text: 'Sản phẩm rất tốt! Da tôi trở nên mềm mại và sáng khỏe hơn nhiều sau khi sử dụng. Mùi hương cũng rất dễ chịu, không gây kích ứng. Sẽ mua lại chắc chắn!',
        images: [
            'https://images.unsplash.com/photo-1556229010-e5ac1b9e5e47?ixlib=rb-4.0.3&auto=format&fit=crop&w=200&q=80',
            'https://images.unsplash.com/photo-1585652757141-032b1374cf12?ixlib=rb-4.0.3&auto=format&fit=crop&w=200&q=80'
        ],
        helpful: 15,
        verified: true
    },
    {
        id: 2,
        userName: 'Thu Hương',
        avatar: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?ixlib=rb-4.0.3&auto=format&fit=crop&w=100&q=80',
        rating: 5,
        date: '2025-08-03',
        text: 'Chất lượng tuyệt vời, hoàn toàn từ thiên nhiên. Không gây bết dính, thấm nhanh vào da. Shop gói hàng cẩn thận, giao hàng nhanh.',
        images: [
            'https://images.unsplash.com/photo-1571781926291-c477ebfd024b?ixlib=rb-4.0.3&auto=format&fit=crop&w=200&q=80'
        ],
        helpful: 12,
        verified: true
    },
    {
        id: 3,
        userName: 'Phương Linh',
        avatar: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?ixlib=rb-4.0.3&auto=format&fit=crop&w=100&q=80',
        rating: 4,
        date: '2025-08-01',
        text: 'Sản phẩm ổn, đúng như mô tả. Da tôi thuộc loại nhạy cảm nhưng dùng không bị kích ứng gì. Giá cả hợp lý. Sẽ theo dõi thêm các sản phẩm khác của shop.',
        images: [],
        helpful: 8,
        verified: true
    },
    {
        id: 4,
        userName: 'Hoàng Nam',
        avatar: 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?ixlib=rb-4.0.3&auto=format&fit=crop&w=100&q=80',
        rating: 5,
        date: '2025-07-30',
        text: 'Mua cho vợ, vợ dùng rất hài lòng. Da trở nên mịn màng hơn hẳn. Mùi hương nhẹ nhàng, không nồng như mấy loại kem khác.',
        images: [
            'https://images.unsplash.com/photo-1596755389378-c31d21fd1273?ixlib=rb-4.0.3&auto=format&fit=crop&w=200&q=80',
            'https://images.unsplash.com/photo-1522335789203-aabd1fc54bc9?ixlib=rb-4.0.3&auto=format&fit=crop&w=200&q=80',
            'https://images.unsplash.com/photo-1559599101-f09722fb4948?ixlib=rb-4.0.3&auto=format&fit=crop&w=200&q=80'
        ],
        helpful: 22,
        verified: true
    },
    {
        id: 5,
        userName: 'Mai Chi',
        avatar: 'https://images.unsplash.com/photo-1502767089025-6572583495b9?ixlib=rb-4.0.3&auto=format&fit=crop&w=100&q=80',
        rating: 4,
        date: '2025-07-28',
        text: 'Sản phẩm khá tốt, thấm vào da nhanh. Tuy nhiên mình thấy hội có hiệu quả nhưng không được nhanh lắm. Có thể do da mình khá khô.',
        images: [],
        helpful: 5,
        verified: false
    },
    {
        id: 6,
        userName: 'Quỳnh Anh',
        avatar: 'https://images.unsplash.com/photo-1534528741775-53994a69daeb?ixlib=rb-4.0.3&auto=format&fit=crop&w=100&q=80',
        rating: 5,
        date: '2025-07-25',
        text: 'Đây là lần thứ 3 mình mua sản phẩm này. Chất lượng ổn định, da mình dùng từ khô chuyển thành da thường. Rất hài lòng với shop!',
        images: [
            'https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?ixlib=rb-4.0.3&auto=format&fit=crop&w=200&q=80'
        ],
        helpful: 18,
        verified: true
    }
];

// Biến để quản lý đánh giá
let currentFilter = 'all';
let displayedReviews = 3;

// Tạo HTML cho đánh giá
function createReviewHTML(review) {
    const stars = '⭐'.repeat(review.rating) + '☆'.repeat(5 - review.rating);
    const formattedDate = new Date(review.date).toLocaleDateString('vi-VN', {
        year: 'numeric',
        month: 'long', 
        day: 'numeric'
    });
    
    const imagesHTML = review.images.length > 0 ? `
        <div class="review-images">
            ${review.images.map(img => `
                <img src="${img}" alt="Review image" class="review-image" onclick="openImageModal('${img}')">
            `).join('')}
        </div>
    ` : '';

    const verifiedHTML = review.verified ? '<span class="verified-purchase">Đã mua hàng</span>' : '';

    return `
        <div class="review-item" data-rating="${review.rating}">
            <div class="review-header">
                <img src="${review.avatar}" alt="${review.userName}" class="reviewer-avatar">
                <div class="reviewer-info">
                    <div class="reviewer-name">${review.userName}</div>
                    <div class="review-date">${formattedDate}</div>
                </div>
                <div class="review-rating">
                    <div class="review-stars">${stars}</div>
                    ${verifiedHTML}
                </div>
            </div>
            <div class="review-content">
                <p class="review-text">${review.text}</p>
                ${imagesHTML}
            </div>
            <div class="review-actions">
                <div class="review-helpful" onclick="toggleHelpful(${review.id})">
                    <i class="far fa-thumbs-up"></i>
                    <span class="helpful-count">Hữu ích (${review.helpful})</span>
                </div>
            </div>
        </div>
    `;
}

// Hiển thị đánh giá
function displayReviews() {
    const reviewsList = document.getElementById('reviewsList');
    if (!reviewsList) return;

    let filteredReviews = reviewsData;
    
    // Lọc theo rating
    if (currentFilter !== 'all') {
        if (currentFilter === 'with-photos') {
            filteredReviews = reviewsData.filter(review => review.images.length > 0);
        } else {
            filteredReviews = reviewsData.filter(review => review.rating == currentFilter);
        }
    }

    // Hiển thị số lượng đánh giá theo filter
    const reviewsToShow = filteredReviews.slice(0, displayedReviews);
    reviewsList.innerHTML = reviewsToShow.map(createReviewHTML).join('');

    // Ẩn/hiện nút "Xem thêm"
    const loadMoreBtn = document.querySelector('.btn-load-more');
    if (loadMoreBtn) {
        loadMoreBtn.style.display = displayedReviews >= filteredReviews.length ? 'none' : 'block';
    }
}

// Xử lý filter đánh giá
function setupReviewFilters() {
    const filterBtns = document.querySelectorAll('.filter-btn');
    filterBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            // Remove active class from all buttons
            filterBtns.forEach(b => b.classList.remove('active'));
            // Add active class to clicked button
            this.classList.add('active');
            
            currentFilter = this.getAttribute('data-filter');
            displayedReviews = 3; // Reset số lượng hiển thị
            displayReviews();
        });
    });
}

// Load thêm đánh giá
function loadMoreReviews() {
    displayedReviews += 3;
    displayReviews();
}

// Toggle helpful
function toggleHelpful(reviewId) {
    const review = reviewsData.find(r => r.id === reviewId);
    if (review) {
        review.helpful += 1;
        displayReviews();
    }
}

// Modal xem ảnh
function openImageModal(imageSrc) {
    const modal = document.getElementById('imageModal');
    const modalImg = document.getElementById('modalImage');
    modal.style.display = 'block';
    modalImg.src = imageSrc;
}

function closeImageModal() {
    const modal = document.getElementById('imageModal');
    modal.style.display = 'none';
}

// Đóng modal khi click outside
window.onclick = function(event) {
    const modal = document.getElementById('imageModal');
    if (event.target == modal) {
        modal.style.display = 'none';
    }
}

// Load thông tin khi trang được tải
document.addEventListener('DOMContentLoaded', function() {
    loadProductInfo();
    displayReviews();
    setupReviewFilters();
});
function increaseQuantity() {
  var quantityInput = document.getElementById('productQuantity');
  var currentValue = parseInt(quantityInput.value);

  if (maxQuantity <= 0) {
    showOutOfStockModal();
    return;
  }
  if (currentValue < maxQuantity) {
    quantityInput.value = currentValue + 1;
  } else {
    showOutOfStockModal();
  }
}
function increaseQuantity() {
    var quantityInput = document.getElementById('productQuantity');
    var currentValue = parseInt(quantityInput.value);

    if (maxQuantity <= 0) {
        showOutOfStockModal();
        return;
    }

    if (currentValue < maxQuantity) {
        quantityInput.value = currentValue + 1;
    } else {
        showOutOfStockModal();
    }
}

function decreaseQuantity() {
    var quantityInput = document.getElementById('productQuantity');
    var currentValue = parseInt(quantityInput.value);

    if (currentValue > 1) {
        quantityInput.value = currentValue - 1;
    }
}

function validateQuantity() {
    var quantityInput = document.getElementById('productQuantity');
    var value = parseInt(quantityInput.value);

    if (isNaN(value) || value < 1) {
        quantityInput.value = 1;
    } else if (value > maxQuantity) {
        quantityInput.value = maxQuantity;
        alert('Không thể chọn quá ' + maxQuantity + ' sản phẩm!');
    }
}
        function addToCart(productId) {
            try {
                var quantity = parseInt(document.getElementById('productQuantity').value) || 1;
                if (maxQuantity <= 0) {
                    showOutOfStockModal();
                    return;
                }
                let cart = JSON.parse(localStorage.getItem('cart')) || [];
                let found = cart.find(item => item.id == productId);
                if (found) {
                    found.quantity += quantity;
                } else {
                    cart.push({ id: productId, quantity: quantity });
                }
                localStorage.setItem('cart', JSON.stringify(cart));
                if (typeof updateCartCount === 'function') updateCartCount();
                if (window.showNotification) {
                    showNotification('Đã thêm vào giỏ hàng!', 'success');
                } else {
                    alert('Đã thêm vào giỏ hàng!');
                }
            } catch (e) {
                alert('Có lỗi xảy ra khi thêm vào giỏ hàng.');
            }
    // Hàm cập nhật số lượng sản phẩm trong giỏ hàng trên header (dự phòng nếu chưa có ở layout)
    function updateCartCount() {
        let cart = JSON.parse(localStorage.getItem('cart')) || [];
        let total = cart.reduce((sum, item) => sum + item.quantity, 0);
        const cartCount = document.getElementById('cart-count');
        if (cartCount) {
            if (total > 0) {
                cartCount.textContent = total;
                cartCount.style.display = 'inline-block';
            } else {
                cartCount.style.display = 'none';
            }
        }
    }
    document.addEventListener('DOMContentLoaded', updateCartCount);
        }

        function buyNow(productId) {
            var quantity = document.getElementById('productQuantity').value;
            
            if (maxQuantity <= 0) {
                showOutOfStockModal();
                return;
            }
            
            // Redirect to checkout with this product
            window.location.href = '@Url.Action("MuaNgay", "ChiTiet", new { area = "KhachHang" })' + '?productId=' + productId + '&quantity=' + quantity;
        }

        function changeMainImage(src) {
            document.getElementById('mainImage').src = src;
            
            // Update active thumbnail
            var thumbnails = document.querySelectorAll('.thumbnail');
            thumbnails.forEach(thumb => thumb.classList.remove('active'));
            event.target.classList.add('active');
        }

        function openImageModal(src) {
            document.getElementById('imageModal').style.display = 'block';
            document.getElementById('modalImage').src = src;
        }

        function closeImageModal() {
            document.getElementById('imageModal').style.display = 'none';
        }

        function loadMoreReviews(productId) {
            // Implementation for loading more reviews
            console.log('Load more reviews for product:', productId);
        }

        function showReviewForm() {
            document.getElementById('btnWriteReview').style.display = 'none';
            document.getElementById('reviewFormContainer').style.display = 'block';
        }

        function hideReviewForm() {
            document.getElementById('btnWriteReview').style.display = 'block';
            document.getElementById('reviewFormContainer').style.display = 'none';
            // Reset form
            document.getElementById('reviewForm').reset();
            document.getElementById('selectedImages').innerHTML = '';
            // Reset file input
            document.getElementById('imageUpload').value = '';
            updateCharCount();
            updateRatingText();
            // Clear rating text
            document.getElementById('ratingText').textContent = '';
            // Hide rating error
            var ratingError = document.getElementById('ratingError');
            if (ratingError) {
                ratingError.style.display = 'none';
            }
        }

        function updateCharCount() {
            var textarea = document.getElementById('comment');
            var charCount = document.getElementById('charCount');
            var maxLength = 2500;
            var currentLength = textarea.value.length;
            
            // Đếm chính xác số ký tự thực tế
            var actualLength = 0;
            for (var i = 0; i < textarea.value.length; i++) {
                actualLength++;
            }
            
            // Không giới hạn tại đây, chỉ hiển thị số đếm
            var remaining = maxLength - actualLength;
            charCount.textContent = remaining;
            
            // Thay đổi màu khi gần hết ký tự
            if (remaining < 100) {
                charCount.style.color = '#e74c3c';
            } else if (remaining < 300) {
                charCount.style.color = '#f39c12';
            } else {
                charCount.style.color = '#666';
            }
        }

        function updateRatingText() {
            var ratingInputs = document.querySelectorAll('input[name="rating"]');
            var ratingText = document.getElementById('ratingText');
            var ratingTexts = {
                '1': 'Rất tệ',
                '2': 'Tệ', 
                '3': 'Bình thường',
                '4': 'Hài lòng',
                '5': 'Rất hài lòng'
            };
            
            ratingInputs.forEach(function(input) {
                input.addEventListener('change', function() {
                    if (this.checked) {
                        ratingText.textContent = ratingTexts[this.value];
                        // Hide rating error when user selects a rating
                        var ratingError = document.getElementById('ratingError');
                        if (ratingError) {
                            ratingError.style.display = 'none';
                        }
                    }
                });
            });
        }

        function validateReviewForm() {
            var ratingInputs = document.querySelectorAll('input[name="rating"]');
            var isRatingSelected = false;
            
            for (var i = 0; i < ratingInputs.length; i++) {
                if (ratingInputs[i].checked) {
                    isRatingSelected = true;
                    break;
                }
            }
            
            var ratingError = document.getElementById('ratingError');
            
            if (!isRatingSelected) {
                ratingError.style.display = 'block';
                ratingError.scrollIntoView({ behavior: 'smooth', block: 'center' });
                return false;
            } else {
                ratingError.style.display = 'none';
            }
            
            return true;
        }

        function handleImageUpload() {
            var input = document.getElementById('imageUpload');
            var preview = document.getElementById('selectedImages');
            
            input.addEventListener('change', function(e) {
                var files = e.target.files;
                var maxFiles = 5;
                
                // Check total files count (existing + new)
                var currentImageCount = preview.querySelectorAll('.preview-image-container').length;
                
                if (currentImageCount + files.length > maxFiles) {
                    alert('Bạn chỉ có thể chọn tối đa ' + maxFiles + ' hình ảnh.');
                    // Reset input
                    this.value = '';
                    return;
                }
                
                // Process each file
                for (var i = 0; i < files.length; i++) {
                    var file = files[i];
                    
                    // Validate file type
                    if (!file.type.startsWith('image/')) {
                        alert('Vui lòng chỉ chọn file hình ảnh.');
                        continue;
                    }
                    
                    // Validate file size (max 5MB)
                    if (file.size > 5 * 1024 * 1024) {
                        alert('Kích thước file không được vượt quá 5MB.');
                        continue;
                    }
                    
                    var reader = new FileReader();
                    reader.onload = function(e) {
                        var imageContainer = document.createElement('div');
                        imageContainer.className = 'preview-image-container';
                        
                        var img = document.createElement('img');
                        img.src = e.target.result;
                        img.className = 'preview-image';
                        img.alt = 'Preview';
                        
                        var removeBtn = document.createElement('button');
                        removeBtn.className = 'remove-image-btn';
                        removeBtn.innerHTML = '×';
                        removeBtn.type = 'button';
                        removeBtn.onclick = function() {
                            imageContainer.remove();
                            // Clear and trigger change to reset FileList
                            var currentFiles = Array.from(input.files);
                            var dt = new DataTransfer();
                            currentFiles.forEach(function(file, index) {
                                if (index !== Array.from(preview.children).indexOf(imageContainer)) {
                                    dt.items.add(file);
                                }
                            });
                            input.files = dt.files;
                        };
                        
                        imageContainer.appendChild(img);
                        imageContainer.appendChild(removeBtn);
                        preview.appendChild(imageContainer);
                    };
                    
                    reader.readAsDataURL(file);
                }
                
                // Clear input to allow selecting same files again if needed
                // Don't clear here as we need the files for form submission
            });
        }

        // Initialize page when DOM is loaded
        document.addEventListener('DOMContentLoaded', function() {
            // Setup click handlers for thumbnails
            var thumbnails = document.querySelectorAll('.thumbnail');
            thumbnails.forEach(function(thumb) {
                thumb.addEventListener('click', function() {
                    changeMainImage(this.src);
                });
            });

            // Setup image modal click handlers
            var mainImage = document.getElementById('mainImage');
            if (mainImage) {
                mainImage.addEventListener('click', function() {
                    openImageModal(this.src);
                });
            }

            // Setup modal close handlers
            var modal = document.getElementById('imageModal');
            if (modal) {
                modal.addEventListener('click', function(e) {
                    if (e.target === this) {
                        closeImageModal();
                    }
                });
            }

            // Setup quantity validation
            var quantityInput = document.getElementById('productQuantity');
            if (quantityInput) {
                quantityInput.addEventListener('blur', validateQuantity);
                quantityInput.addEventListener('input', validateQuantity);
            }

            // Setup image upload handler
            handleImageUpload();

            // Setup rating text handler
            updateRatingText();

            // Setup review filter buttons
            var filterButtons = document.querySelectorAll('.filter-btn');
            filterButtons.forEach(function(btn) {
                btn.addEventListener('click', function() {
                    // Remove active class from all buttons
                    filterButtons.forEach(b => b.classList.remove('active'));
                    // Add active class to clicked button
                    this.classList.add('active');
                    
                    var filter = this.getAttribute('data-filter');
                    var reviews = document.querySelectorAll('.review-item');
                    
                    reviews.forEach(function(review) {
                        if (filter === 'all' || review.getAttribute('data-rating') === filter) {
                            review.style.display = 'block';
                        } else {
                            review.style.display = 'none';
                        }
                    });
                });
            });

            // Add client-side paging for reviews: show 5, load 5 more on click
            try {
                var reviews = Array.from(document.querySelectorAll('#reviewsList .review-item'));
                var per = 5;
                var shown = per;

                if (!reviews || reviews.length === 0) return;

                // Remove any leftover server-rendered 'load more' buttons that don't have our id
                var legacyBtns = Array.from(document.querySelectorAll('.btn-load-more'));
                legacyBtns.forEach(function(b) {
                    if (!b.id || b.id !== 'btnLoadMoreReviews') {
                        // remove the button element (and its wrapper if present)
                        var parent = b.parentNode;
                        b.remove();
                        // if parent now empty and has class 'load-more-reviews', remove it too
                        if (parent && parent.classList && parent.classList.contains('load-more-reviews') && parent.children.length === 0) {
                            parent.remove();
                        }
                    }
                });

                // Initially hide reviews beyond the first `per`
                reviews.forEach(function(r, i) {
                    if (i >= per) r.style.display = 'none';
                });

                // If there are more than `per` reviews, add a Load More button
                if (reviews.length > per) {
                    // Create button if not already present
                    if (!document.getElementById('btnLoadMoreReviews')) {
                        var btn = document.createElement('button');
                        btn.id = 'btnLoadMoreReviews';
                        btn.type = 'button';
                        btn.className = 'btn-load-more';
                        btn.textContent = 'Xem thêm đánh giá';

                        // Append button after the reviews list if possible
                        var reviewsList = document.getElementById('reviewsList');
                        if (reviewsList && reviewsList.parentNode) {
                            var wrapper = document.createElement('div');
                            wrapper.className = 'load-more-reviews';
                            wrapper.style.textAlign = 'center';
                            wrapper.style.marginTop = '1rem';
                            wrapper.appendChild(btn);
                            reviewsList.parentNode.insertBefore(wrapper, reviewsList.nextSibling);
                        } else {
                            // fallback: append to product-reviews container
                            var container = document.querySelector('.product-reviews');
                            if (container) container.appendChild(btn);
                        }

                        btn.addEventListener('click', function() {
                            var next = Math.min(shown + per, reviews.length);
                            for (var i = shown; i < next; i++) {
                                if (reviews[i]) reviews[i].style.display = 'block';
                            }
                            shown = next;
                            if (shown >= reviews.length) {
                                btn.style.display = 'none';
                            }
                        });
                    }
                }
            } catch (e) {
                // Fail silently to avoid breaking the page
                console.error('Error initializing review pager:', e);
            }
        });

        // Out of Stock Modal Functions
        function showOutOfStockModal() {
            document.getElementById('outOfStockModal').style.display = 'flex';
            document.body.style.overflow = 'hidden'; // Prevent scrolling
        }

        function closeOutOfStockModal() {
            document.getElementById('outOfStockModal').style.display = 'none';
            document.body.style.overflow = 'auto'; // Restore scrolling
        }

        // Close modal when pressing Escape key
        document.addEventListener('keydown', function(event) {
            if (event.key === 'Escape') {
                closeOutOfStockModal();
            }
        });