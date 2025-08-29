document.addEventListener("DOMContentLoaded", function () {
  // Initialize footer link notifications
  initFooterNotifications();
  // Initialize social media notifications
  initSocialMediaNotifications();
});

function initFooterNotifications() {
  // Create notification container if it doesn't exist
  createNotificationContainer();

  // Add click event listeners to footer links
  const footerLinks = document.querySelectorAll(
    ".footer-links a[data-feature]"
  );

  footerLinks.forEach((link) => {
    link.addEventListener("click", function (e) {
      e.preventDefault();

      const featureName =
        this.querySelector("i").nextSibling.textContent.trim();
      showDevelopmentNotification(featureName);
    });
  });
}

function initSocialMediaNotifications() {
  // Create notification container if it doesn't exist
  createNotificationContainer();

  // Add click event listeners to social media links
  const socialLinks = document.querySelectorAll(".social-link[data-social]");

  socialLinks.forEach((link) => {
    link.addEventListener("click", function (e) {
      e.preventDefault();

      const socialPlatform = this.dataset.social;
      const platformName = this.title || socialPlatform;
      showSocialMediaNotification(platformName, socialPlatform);
    });
  });
}

function createNotificationContainer() {
  if (document.getElementById("notification-container")) return;

  const container = document.createElement("div");
  container.id = "notification-container";
  container.className = "notification-container";
  document.body.appendChild(container);
}

function showDevelopmentNotification(featureName) {
  const container = document.getElementById("notification-container");

  // Create notification element
  const notification = document.createElement("div");
  notification.className = "development-notification";

  notification.innerHTML = `
        <div class="notification-content">
            <div class="notification-header">
                <i class="fas fa-tools notification-icon"></i>
                <span class="notification-title">Tính năng đang phát triển</span>
                <button class="notification-close" onclick="closeNotification(this)">
                    <i class="fas fa-times"></i>
                </button>
            </div>
            <div class="notification-body">
                <p><strong>"${featureName}"</strong> hiện đang trong quá trình phát triển.</p>
                <p>Chúng tôi sẽ sớm ra mắt tính năng này trong thời gian tới. Cảm ơn bạn đã quan tâm!</p>
            </div>
            <div class="notification-footer">
                <i class="fas fa-heart text-danger"></i>
                <span>LittleFish Beauty Team</span>
            </div>
        </div>
    `;

  // Add to container
  container.appendChild(notification);

  // Trigger animation
  setTimeout(() => {
    notification.classList.add("show");
  }, 100);

  // Auto remove after 5 seconds
  setTimeout(() => {
    closeNotification(notification.querySelector(".notification-close"));
  }, 2000);
}

function showSocialMediaNotification(platformName, socialPlatform) {
  const container = document.getElementById("notification-container");

  // Get platform-specific icon and color
  const platformInfo = getSocialPlatformInfo(socialPlatform);

  // Create notification element
  const notification = document.createElement("div");
  notification.className = "development-notification social-notification";

  notification.innerHTML = `
        <div class="notification-content">
            <div class="notification-header">
                <i class="${platformInfo.icon} notification-icon" style="color: ${platformInfo.color}"></i>
                <span class="notification-title">Kết nối ${platformName}</span>
                <button class="notification-close" onclick="closeNotification(this)">
                    <i class="fas fa-times"></i>
                </button>
            </div>
            <div class="notification-body">
                <p>Trang <strong>${platformName}</strong> của chúng tôi đang được xây dựng.</p>
                <p>Hãy theo dõi để không bỏ lỡ những chia sẻ thú vị về làm đẹp và các ưu đãi độc quyền!</p>
                <div class="social-preview">
                    <i class="${platformInfo.icon}" style="color: ${platformInfo.color}"></i>
                    <span>@LittleFishBeauty</span>
                </div>
            </div>
            <div class="notification-footer">
                <i class="fas fa-rocket"></i>
                <span>Sắp ra mắt!</span>
            </div>
        </div>
    `;

  // Add to container
  container.appendChild(notification);

  // Trigger animation
  setTimeout(() => {
    notification.classList.add("show");
  }, 100);

  // Auto remove after 6 seconds (longer for social media)
  setTimeout(() => {
    closeNotification(notification.querySelector(".notification-close"));
  }, 2000);
}

function getSocialPlatformInfo(platform) {
  const platformMap = {
    facebook: {
      icon: "fab fa-facebook-f",
      color: "#1877F2",
    },
    instagram: {
      icon: "fab fa-instagram",
      color: "#E4405F",
    },
    youtube: {
      icon: "fab fa-youtube",
      color: "#FF0000",
    },
    tiktok: {
      icon: "fab fa-tiktok",
      color: "#000000",
    },
    zalo: {
      icon: "fas fa-comments",
      color: "#0068FF",
    },
  };

  return (
    platformMap[platform] || {
      icon: "fas fa-share-alt",
      color: "#4CAF50",
    }
  );
}

function closeNotification(closeBtn) {
  const notification = closeBtn.closest(".development-notification");

  notification.classList.add("hide");

  setTimeout(() => {
    if (notification.parentNode) {
      notification.parentNode.removeChild(notification);
    }
  }, 300);
}

// Close notification when clicking outside
document.addEventListener("click", function (e) {
  if (e.target.classList.contains("development-notification")) {
    closeNotification(e.target.querySelector(".notification-close"));
  }
});

// Close notification with Escape key
document.addEventListener("keydown", function (e) {
  if (e.key === "Escape") {
    const notifications = document.querySelectorAll(
      ".development-notification"
    );
    notifications.forEach((notification) => {
      const closeBtn = notification.querySelector(".notification-close");
      if (closeBtn) {
        closeNotification(closeBtn);
      }
    });
  }
});
