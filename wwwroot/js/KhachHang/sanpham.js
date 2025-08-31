// Quản lý lịch sử tìm kiếm
let searchHistory = JSON.parse(localStorage.getItem('searchHistory')) || [];

// Elements
const searchInput = document.getElementById('searchInput');
const searchBtn = document.getElementById('searchBtn');
const searchSuggestions = document.getElementById('searchSuggestions');
const searchFilters = document.getElementById('searchFilters');
const advancedSearchToggle = document.getElementById('advancedSearchToggle');

// Hiển thị gợi ý tìm kiếm
function showSearchSuggestions(query = '') {
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
                searchInput.value = term;
                performSearch(term);
                hideSearchSuggestions();
            };
            historyItemsInner.appendChild(historyItem);
        });
    } else {
        // TODO: Integrate with real product API for search suggestions
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
    searchSuggestions.style.display = 'none';
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
        
        // TODO: Integrate with real search API
        // Hiển thị kết quả tìm kiếm
        console.log(`Searching for: "${query}"`);
        // Here you would typically make an API call to search products
        // Example: await searchProducts(query);
        
        // For now, show a placeholder message
        alert(`Tìm kiếm: "${query}"\nChức năng tìm kiếm sẽ được tích hợp với database`);
    }
}

// TODO: Replace with actual API call
function getSearchResults(query) {
    // This should be replaced with actual API call to backend
    console.log(`Getting search results for: ${query}`);
    return [];
}

function filterByCategory(category) {
    // TODO: Integrate with real product filtering API
    console.log(`Filtering by category: ${category}`);
    // Loại bỏ alert để không hiện thông báo
}

function clearSearchHistory() {
    searchHistory = [];
    localStorage.removeItem('searchHistory');
    hideSearchSuggestions();
}

// Search Event listeners
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
        performSearch(searchInput.value);
        hideSearchSuggestions();
    });
}

// Toggle advanced search
if (advancedSearchToggle) {
    advancedSearchToggle.addEventListener('click', (e) => {
        e.preventDefault();
        const isVisible = searchFilters.style.display === 'block';
        searchFilters.style.display = isVisible ? 'none' : 'block';
        hideSearchSuggestions();
    });
}

// Hide suggestions when clicking outside
document.addEventListener('click', (e) => {
    if (!e.target.closest('.search-container')) {
        hideSearchSuggestions();
        if (searchFilters) searchFilters.style.display = 'none';
    }
});

// Advanced search filters
const applyFiltersBtn = document.getElementById('applyFilters');
const priceSlider = document.getElementById('priceSlider');
const priceValue = document.getElementById('priceValue');

// Price slider functionality
if (priceSlider && priceValue) {
    priceSlider.addEventListener('input', (e) => {
        const value = parseInt(e.target.value);
        if (value >= 500000) {
            priceValue.textContent = '500.000₫+';
        } else {
            priceValue.textContent = value.toLocaleString('vi-VN') + '₫';
        }
    });
}

if (applyFiltersBtn) {
    applyFiltersBtn.addEventListener('click', () => {
        const category = document.getElementById('categoryFilter').value;
        const sortBy = document.getElementById('sortBy').value;
        const maxPrice = parseInt(document.getElementById('priceSlider').value);
        
        // TODO: Integrate with real product filtering API
        console.log('Applied filters:', { category, sortBy, maxPrice });
        
        // Here you would typically make an API call with these filters
        // Example: await filterProducts({ category, sortBy, maxPrice });
        
        const categoryText = category ? ` trong danh mục ${getCategoryName(category)}` : '';
        const priceText = maxPrice < 500000 ? ` với giá tối đa ${maxPrice.toLocaleString('vi-VN')}₫` : '';
        
        alert(`Áp dụng bộ lọc${categoryText}${priceText}\nChức năng lọc sẽ được tích hợp với database`);
        if (searchFilters) searchFilters.style.display = 'none';
    });
}

// Category dropdown navigation đã được xử lý trong phần dropdown menu navbar ở trên

// Existing sanpham.html functionality
// Add to cart functionality
document.querySelectorAll('.btn-add-cart').forEach(btn => {
    btn.addEventListener('click', function() {
        // Show loading state
        const originalText = this.innerHTML;
        this.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Đang thêm...';
        this.disabled = true;

        // Simulate API call
        setTimeout(() => {
            this.innerHTML = '<i class="fas fa-check"></i> Đã thêm!';
            this.style.backgroundColor = '#27ae60';
            
            setTimeout(() => {
                this.innerHTML = originalText;
                this.style.backgroundColor = '';
                this.disabled = false;
            }, 2000);
        }, 1000);
    });
});

// View toggle functionality
document.querySelectorAll('.view-btn').forEach(btn => {
    btn.addEventListener('click', function() {
        document.querySelectorAll('.view-btn').forEach(b => b.classList.remove('active'));
        this.classList.add('active');
    });
});

// Category filter
document.querySelectorAll('.category-list a').forEach(link => {
    link.addEventListener('click', function(e) {
        e.preventDefault();
        document.querySelectorAll('.category-list a').forEach(l => l.classList.remove('active'));
        this.classList.add('active');
        
        // Show loading
        document.querySelector('.loading').style.display = 'block';
        document.getElementById('products-container').style.opacity = '0.5';
        
        // Simulate filter
        setTimeout(() => {
            document.querySelector('.loading').style.display = 'none';
            document.getElementById('products-container').style.opacity = '1';
        }, 1500);
    });
});

// Sort functionality
document.querySelector('.sort-dropdown').addEventListener('change', function() {
    document.querySelector('.loading').style.display = 'block';
    document.getElementById('products-container').style.opacity = '0.5';
    
    setTimeout(() => {
        document.querySelector('.loading').style.display = 'none';
        document.getElementById('products-container').style.opacity = '1';
    }, 1000);
});

// Price filter
document.querySelector('.filter-section .btn').addEventListener('click', function() {
    document.querySelector('.loading').style.display = 'block';
    document.getElementById('products-container').style.opacity = '0.5';
    
    setTimeout(() => {
        document.querySelector('.loading').style.display = 'none';
        document.getElementById('products-container').style.opacity = '1';
    }, 1200);
});

// Function to view product detail
function viewProductDetail(productId) {
    // Chuyển đến trang chi tiết sản phẩm với ID
    window.location.href = `chitiet-sanpham.html?id=${productId}`;
}

// View Toggle Functionality
document.addEventListener('DOMContentLoaded', function() {
    const viewButtons = document.querySelectorAll('.view-btn');
    const productsContainer = document.getElementById('products-container');
    
    viewButtons.forEach(button => {
        button.addEventListener('click', function() {
            // Remove active class from all buttons
            viewButtons.forEach(btn => btn.classList.remove('active'));
            // Add active class to clicked button
            this.classList.add('active');
            
            // Check which button was clicked
            if (this.innerHTML.includes('fa-th')) {
                // Grid view
                productsContainer.classList.remove('list-view');
                const productCards = productsContainer.querySelectorAll('[class*="col-"]');
                productCards.forEach(card => {
                    card.className = 'col-lg-4 col-md-6 mb-4';
                });
            } else {
                // List view
                productsContainer.classList.add('list-view');
                const productCards = productsContainer.querySelectorAll('[class*="col-lg-4"]');
                productCards.forEach(card => {
                    card.className = 'col-12 mb-3';
                });
            }
        });
    });

    // Thêm chức năng click vào card sản phẩm để chuyển đến trang chi tiết
    const productCards = document.querySelectorAll('.product-card');
    productCards.forEach((card, index) => {
        card.style.cursor = 'pointer';
        card.addEventListener('click', function(e) {
            // Ngăn chặn event bubbling từ các button bên trong
            if (e.target.closest('.product-actions') || e.target.closest('.product-overlay')) {
                return;
            }
            
            // Chuyển đến trang chi tiết với ID sản phẩm
            const productId = index + 1; // ID từ 1-6 cho các sản phẩm hiện tại
            window.location.href = `chitiet-sanpham.html?id=${productId}`;
        });
    });
});

// Biến lưu trạng thái lọc hiện tại
let currentFilters = {
    category: 'all',
    minPrice: 0,
    maxPrice: Infinity,
    sortBy: 'default',
    isNew: false
};

// Hiển thị sản phẩm
function displayProducts(products) {
    const container = document.getElementById('products-container');
    container.innerHTML = '';

    // Kiểm tra nếu không có sản phẩm
    if (products.length === 0) {
        let message = 'Hiện tại chưa có sản phẩm!';
        let description = 'Vui lòng thử lại với bộ lọc khác hoặc xem tất cả sản phẩm.';
        
        // Thông báo đặc biệt cho sản phẩm mới
        if (currentFilters.isNew) {
            message = 'Chưa có sản phẩm mới!';
            description = 'Hiện tại chưa có sản phẩm nào được thêm trong 24h qua. Hãy quay lại sau nhé!';
        }
        
        container.innerHTML = `
            <div class="col-12">
                <div class="no-products-message text-center py-5">
                    <i class="fas fa-box-open" style="font-size: 4rem; color: #ccc; margin-bottom: 20px;"></i>
                    <h4 style="color: #666; margin-bottom: 10px;">${message}</h4>
                    <p style="color: #999;">${description}</p>
                    <button class="btn btn-outline-success mt-3" onclick="resetFilters()">
                        <i class="fas fa-refresh me-2"></i>Xem tất cả sản phẩm
                    </button>
                </div>
            </div>
        `;
        return;
    }

    products.forEach(product => {
        const productCard = createProductCard(product);
        container.appendChild(productCard);
    });

    // Thêm lại event listeners cho các card mới
    addProductCardListeners();
}

// Tạo HTML cho một sản phẩm
function createProductCard(product) {
    const col = document.createElement('div');
    col.className = 'col-lg-4 col-md-6 mb-4';

    const stars = '⭐'.repeat(product.rating) + '☆'.repeat(5 - product.rating);
    const badgeHtml = product.badge ? `<span class="badge badge-${product.badge.toLowerCase().replace(' ', '-')}">${product.badge}</span>` : '';
    
    col.innerHTML = `
        <div class="product-card" data-product-id="${product.id}">
            <div class="product-image-container">
                <img src="${product.image}" class="product-image" alt="${product.name}">
                ${badgeHtml}
                <div class="product-overlay">
                    <a href="chitiet-sanpham.html?id=${product.id}" class="btn btn-outline-light btn-sm">Xem chi tiết</a>
                </div>
            </div>
            <div class="product-info">
                <h6 class="product-name">${product.name}</h6>
                <div class="product-rating">
                    ${stars.split('').map(star => `<span class="star${star === '⭐' ? ' filled' : ''}">${star}</span>`).join('')}
                    <span class="review-count">(${product.reviewCount})</span>
                </div>
                <div class="product-price">
                    <span class="current-price">${formatCurrency(product.price)}</span>
                </div>
                <div class="product-actions">
                    <button class="btn btn-success btn-sm flex-fill">
                        <i class="fas fa-shopping-cart me-1"></i>Thêm giỏ hàng
                    </button>
                </div>
            </div>
        </div>
    `;

    return col;
}

// Format tiền tệ
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN').format(amount) + '₫';
}

// Lọc sản phẩm
function filterProducts() {
    let filteredProducts = [...allProducts];

    // Lọc sản phẩm mới (nếu được yêu cầu)
    if (currentFilters.isNew) {
        const twentyFourHoursAgo = new Date();
        twentyFourHoursAgo.setHours(twentyFourHoursAgo.getHours() - 24);
        
        filteredProducts = filteredProducts.filter(product => {
            if (product.createdAt) {
                const productDate = new Date(product.createdAt);
                return productDate > twentyFourHoursAgo;
            }
            return false;
        });
    }

    // Lọc theo danh mục (chỉ khi không lọc sản phẩm mới)
    if (!currentFilters.isNew && currentFilters.category !== 'all') {
        filteredProducts = filteredProducts.filter(product => product.category === currentFilters.category);
    }

    // Lọc theo giá
    filteredProducts = filteredProducts.filter(product => 
        product.price >= currentFilters.minPrice && product.price <= currentFilters.maxPrice
    );

    // Sắp xếp
    switch (currentFilters.sortBy) {
        case 'price-low-high':
            filteredProducts.sort((a, b) => a.price - b.price);
            break;
        case 'price-high-low':
            filteredProducts.sort((a, b) => b.price - a.price);
            break;
        case 'rating':
            filteredProducts.sort((a, b) => b.rating - a.rating);
            break;
        case 'newest':
            filteredProducts.sort((a, b) => {
                if (a.createdAt && b.createdAt) {
                    return new Date(b.createdAt) - new Date(a.createdAt);
                }
                return b.id - a.id;
            });
            break;
        case 'popular':
            filteredProducts.sort((a, b) => b.reviewCount - a.reviewCount);
            break;
        case 'bestseller':
            filteredProducts.sort((a, b) => {
                if (a.badge === 'Best Seller' && b.badge !== 'Best Seller') return -1;
                if (a.badge !== 'Best Seller' && b.badge === 'Best Seller') return 1;
                return b.reviewCount - a.reviewCount;
            });
            break;
    }

    displayProducts(filteredProducts);
    updateProductCount(filteredProducts.length);
}

// Cập nhật số lượng sản phẩm hiển thị
function updateProductCount(count) {
    const countElement = document.querySelector('.controls-bar span');
    if (countElement) {
        if (count === 0) {
            countElement.textContent = 'Không tìm thấy sản phẩm nào';
        } else {
            countElement.textContent = `Hiển thị 1-${count} trong ${count} sản phẩm`;
        }
    }
}

// Thêm event listeners cho product cards
function addProductCardListeners() {
    const productCards = document.querySelectorAll('.product-card');
    productCards.forEach((card) => {
        card.style.cursor = 'pointer';
        card.addEventListener('click', function(e) {
            if (e.target.closest('.product-actions') || e.target.closest('.product-overlay')) {
                return;
            }
            
            const productId = this.getAttribute('data-product-id');
            window.location.href = `chitiet-sanpham.html?id=${productId}`;
        });
    });
}

// Reset tất cả bộ lọc về mặc định
function resetFilters() {
    // Reset các biến filter
    currentFilters = {
        category: 'all',
        minPrice: 0,
        maxPrice: Infinity,
        sortBy: 'default',
        isNew: false
    };

    // Reset URL về trang cơ bản
    window.history.pushState({}, '', 'sanpham.html');
    
    // Reset tiêu đề trang
    const pageTitle = document.querySelector('.page-title, h1');
    if (pageTitle) {
        pageTitle.textContent = 'Sản phẩm';
    }

    // Reset giao diện
    // Reset danh mục
    const categoryList = document.querySelector('.category-list');
    if (categoryList) {
        categoryList.querySelectorAll('a').forEach(a => a.classList.remove('active'));
        const allCategoryLink = categoryList.querySelector('a[data-category="all"]');
        if (allCategoryLink) {
            allCategoryLink.classList.add('active');
        }
    }

    // Reset khoảng giá
    const priceInputs = document.querySelectorAll('.price-input');
    priceInputs.forEach(input => input.value = '');

    // Reset dropdown sắp xếp
    const sortDropdown = document.querySelector('.sort-dropdown');
    if (sortDropdown) {
        sortDropdown.value = 'default';
    }

    // Hiển thị lại tất cả sản phẩm
    displayProducts(allProducts);
    updateProductCount(allProducts.length);
}

// Khởi tạo khi trang load xong
document.addEventListener('DOMContentLoaded', function() {
    // Hiển thị tất cả sản phẩm ban đầu
    displayProducts(allProducts);

    // Kiểm tra URL parameters để lọc theo danh mục hoặc sản phẩm mới
    const urlParams = new URLSearchParams(window.location.search);
    const categoryParam = urlParams.get('category');
    const isNewParam = urlParams.get('new');
    
    if (isNewParam === 'true') {
        // Lọc sản phẩm mới (được tạo trong 24h)
        currentFilters.isNew = true;
        filterProducts();
        
        // Cập nhật tiêu đề trang
        const pageTitle = document.querySelector('.page-title, h1');
        if (pageTitle) {
            pageTitle.textContent = 'Sản phẩm mới';
        }
    } else if (categoryParam) {
        currentFilters.category = categoryParam;
        filterProducts();
    }

    // Danh mục sidebar
    const categoryList = document.querySelector('.category-list');
    if (categoryList) {
        categoryList.innerHTML = `
            <li><a href="#" data-category="all"><i class="fas fa-leaf"></i>Tất Cả Sản Phẩm</a></li>
            <li><a href="#" data-category="skincare"><i class="fas fa-spa"></i>Chăm Sóc Da</a></li>
            <li><a href="#" data-category="makeup"><i class="fas fa-palette"></i>Trang Điểm</a></li>
            <li><a href="#" data-category="haircare"><i class="fas fa-cut"></i>Dưỡng Tóc</a></li>
            <li><a href="#" data-category="suncare"><i class="fas fa-sun"></i>Chống Nắng</a></li>
            <li><a href="#" data-category="essential-oils"><i class="fas fa-seedling"></i>Tinh Dầu</a></li>
            <li><a href="#" data-category="body-care"><i class="fas fa-heart"></i>Chăm Sóc Cơ Thể</a></li>
            <li><a href="#" data-special="new" class="new-products-link"><i class="fas fa-star"></i>Sản Phẩm Mới</a></li>
        `;

        // Set active state dựa trên URL parameter hoặc mặc định
        if (isNewParam === 'true') {
            const newProductsLink = categoryList.querySelector('.new-products-link');
            if (newProductsLink) {
                newProductsLink.classList.add('active');
            }
        } else {
            const activeCategory = categoryParam || 'all';
            const activeLink = categoryList.querySelector(`a[data-category="${activeCategory}"]`);
            if (activeLink) {
                activeLink.classList.add('active');
            } else {
                // Fallback to "all" if category not found
                categoryList.querySelector('a[data-category="all"]').classList.add('active');
            }
        }

        // Event listeners cho danh mục
        categoryList.addEventListener('click', function(e) {
            e.preventDefault();
            const link = e.target.closest('a');
            if (link) {
                // Cập nhật active state
                categoryList.querySelectorAll('a').forEach(a => a.classList.remove('active'));
                link.classList.add('active');

                // Kiểm tra nếu click vào "Sản phẩm mới"
                if (link.hasAttribute('data-special') && link.getAttribute('data-special') === 'new') {
                    // Reset filters và set isNew = true
                    currentFilters = {
                        category: 'all',
                        minPrice: 0,
                        maxPrice: Infinity,
                        sortBy: 'default',
                        isNew: true
                    };
                    filterProducts();
                    
                    // Cập nhật URL và tiêu đề
                    window.history.pushState({}, '', 'sanpham.html?new=true');
                    const pageTitle = document.querySelector('.page-title, h1');
                    if (pageTitle) {
                        pageTitle.textContent = 'Sản phẩm mới';
                    }
                } else {
                    // Xử lý danh mục thông thường
                    currentFilters.isNew = false;
                    currentFilters.category = link.getAttribute('data-category');
                    filterProducts();
                    
                    // Cập nhật URL và tiêu đề
                    const newCategory = link.getAttribute('data-category');
                    if (newCategory === 'all') {
                        window.history.pushState({}, '', 'sanpham.html');
                    } else {
                        window.history.pushState({}, '', `sanpham.html?category=${newCategory}`);
                    }
                    
                    const pageTitle = document.querySelector('.page-title, h1');
                    if (pageTitle) {
                        pageTitle.textContent = 'Sản phẩm';
                    }
                }
            }
        });
    }

    // Bộ lọc giá
    const priceApplyBtn = document.querySelector('.filter-section .btn');
    if (priceApplyBtn) {
        priceApplyBtn.addEventListener('click', function() {
            const minPriceInput = document.querySelector('.price-input:first-of-type');
            const maxPriceInput = document.querySelector('.price-input:last-of-type');

            const minPrice = parseInt(minPriceInput.value) * 1000 || 0;
            const maxPrice = parseInt(maxPriceInput.value) * 1000 || Infinity;

            currentFilters.minPrice = minPrice;
            currentFilters.maxPrice = maxPrice;
            filterProducts();
        });
    }

    // Sắp xếp
    const sortDropdown = document.querySelector('.sort-dropdown');
    if (sortDropdown) {
        sortDropdown.innerHTML = `
            <option value="default">Sắp xếp theo</option>
            <option value="popular">Phổ biến</option>
            <option value="newest">Mới nhất</option>
            <option value="price-low-high">Giá thấp đến cao</option>
            <option value="price-high-low">Giá cao đến thấp</option>
            <option value="bestseller">Bán chạy</option>
            <option value="rating">Đánh giá cao</option>
        `;

        sortDropdown.addEventListener('change', function() {
            currentFilters.sortBy = this.value;
            filterProducts();
        });
    }

    // Dropdown menu trong navbar - chuyển trang hoặc lọc ngay lập tức
    const navDropdownItems = document.querySelectorAll('.navbar .dropdown-item[data-category]');
    
    navDropdownItems.forEach(item => {
        item.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            const category = this.getAttribute('data-category');
            
            // Nếu đã ở trang sản phẩm, chỉ cần lọc
            currentFilters.category = category;
            filterProducts();
            
            // Cập nhật active state trong sidebar
            const categoryList = document.querySelector('.category-list');
            if (categoryList) {
                categoryList.querySelectorAll('a').forEach(a => a.classList.remove('active'));
                const activeLink = categoryList.querySelector(`a[data-category="${category}"]`);
                if (activeLink) {
                    activeLink.classList.add('active');
                }
            }

            // Cập nhật URL
            if (category === 'all') {
                window.history.pushState({}, '', 'sanpham.html');
            } else {
                window.history.pushState({}, '', `sanpham.html?category=${category}`);
            }
        });
    });
});

// Price Range Slider functionality
document.addEventListener('DOMContentLoaded', function() {
    const priceSlider = document.getElementById('priceSlider');
    const priceDisplay = document.getElementById('priceRangeDisplay');
    const maxPriceInput = document.getElementById('maxPriceValue');
    
    // Only initialize if elements exist
    if (priceSlider && priceDisplay && maxPriceInput) {
        function formatPrice(price) {
            return price.toLocaleString('vi-VN') + '₫';
        }
        
        function updatePriceDisplay() {
            const maxPrice = parseInt(priceSlider.value);
            const minPrice = 0;
            
            // Update display text
            priceDisplay.textContent = formatPrice(minPrice) + ' - ' + formatPrice(maxPrice);
            
            // Update hidden input
            maxPriceInput.value = maxPrice;
        }
        
        // Add event listener for real-time updates
        priceSlider.addEventListener('input', updatePriceDisplay);
        
        // Initial display update
        updatePriceDisplay();
    }
});

// Price slider functionality
function initializePriceSlider() {
    const priceSlider = document.getElementById('priceSlider');
    const priceDisplay = document.getElementById('priceRangeDisplay');
    const maxPriceValue = document.getElementById('maxPriceValue');
    
    if (priceSlider && priceDisplay && maxPriceValue) {
        // Set initial display
        const initialValue = parseInt(priceSlider.value);
        const formattedValue = initialValue.toLocaleString('vi-VN');
        priceDisplay.textContent = `0₫ - ${formattedValue}₫`;
        
        priceSlider.addEventListener('input', function(e) {
            const value = parseInt(e.target.value);
            const formattedValue = value.toLocaleString('vi-VN');
            priceDisplay.textContent = `0₫ - ${formattedValue}₫`;
            maxPriceValue.value = value;
        });
    }
}

// Function to add product to cart from product list
function addToCartFromProductList(productId) {
    // Disable button to prevent double clicks
    const button = document.querySelector(`[data-product-id="${productId}"]`);
    if (button) {
        button.disabled = true;
        button.innerHTML = '<i class="fas fa-spinner fa-spin me-1"></i>Đang thêm...';
    }

    fetch('/KhachHang/Cart/AddToCart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: JSON.stringify({
            id: productId,
            quantity: 1
        })
    })
    .then(response => response.json())
    .then(data => {
        // Re-enable button first
        if (button) {
            button.disabled = false;
            button.innerHTML = '<i class="fas fa-shopping-cart me-1"></i>Thêm giỏ hàng';
        }

        if (data.success) {
            // Update cart count in header using global function
            if (typeof updateCartCount === 'function') {
                updateCartCount(data.cartCount);
            } else {
                // Fallback if function not available
                const cartCountElement = document.getElementById('cartCount');
                if (cartCountElement) {
                    cartCountElement.textContent = data.cartCount;
                }
            }
            
            // Show success message
            showSuccessMessage(data.message || 'Đã thêm sản phẩm vào giỏ hàng!');
        } else if (data.requireLogin) {
            // Show login required modal
            const loginModal = new bootstrap.Modal(document.getElementById('loginRequiredModal'));
            loginModal.show();
        } else {
            // Show error message
            showErrorMessage(data.message || 'Có lỗi xảy ra khi thêm sản phẩm!');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        // Re-enable button
        if (button) {
            button.disabled = false;
            button.innerHTML = '<i class="fas fa-shopping-cart me-1"></i>Thêm giỏ hàng';
        }
        showErrorMessage('Có lỗi xảy ra khi thêm sản phẩm!');
    });
}

// Success message function
function showSuccessMessage(message) {
    const toast = createToast('success', message, 'fas fa-check-circle');
    showToast(toast);
}

// Error message function
function showErrorMessage(message) {
    const toast = createToast('error', message, 'fas fa-exclamation-circle');
    showToast(toast);
}

// Create toast notification
function createToast(type, message, iconClass) {
    const toast = document.createElement('div');
    toast.className = `toast-notification ${type}`;
    toast.innerHTML = `
        <div class="toast-content">
            <i class="${iconClass}"></i>
            <span>${message}</span>
        </div>
    `;
    return toast;
}

// Show toast notification
function showToast(toast) {
    document.body.appendChild(toast);
    
    // Show toast
    setTimeout(() => {
        toast.style.transform = 'translateX(0)';
    }, 100);
    
    // Hide toast after 3 seconds
    setTimeout(() => {
        toast.style.transform = 'translateX(100%)';
        setTimeout(() => {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 300);
    }, 3000);
}

// View toggle functionality
function toggleView(viewType) {
    const productContainer = document.getElementById('products-container');
    const gridBtn = document.querySelector('.view-btn.grid-view');
    const listBtn = document.querySelector('.view-btn.list-view');
    
    if (viewType === 'grid') {
        productContainer.classList.remove('list-view');
        productContainer.classList.add('grid-view');
        if (gridBtn) gridBtn.classList.add('active');
        if (listBtn) listBtn.classList.remove('active');
        
        // Update product items for grid view
        document.querySelectorAll('.product-item').forEach(item => {
            item.className = 'col-lg-4 col-md-6 mb-4 product-item';
        });
    } else if (viewType === 'list') {
        productContainer.classList.remove('grid-view');
        productContainer.classList.add('list-view');
        if (listBtn) listBtn.classList.add('active');
        if (gridBtn) gridBtn.classList.remove('active');
        
        // Update product items for list view - compact text-only layout
        document.querySelectorAll('.product-item').forEach(item => {
            item.className = 'col-12 mb-2 product-item';
        });
    }
}

// Initialize all functionality when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Initialize price slider
    initializePriceSlider();
    
    // Set default to grid view
    toggleView('grid');
    
    // Add other existing initializations here
    // ...existing code...
});

// Legacy function for compatibility
function addToCart(productId) {
    console.log('Thêm sản phẩm ' + productId + ' vào giỏ hàng');
    addToCartFromProductList(productId);
}