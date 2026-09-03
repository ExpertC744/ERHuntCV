/* ========================================
   HuntCV - Interactions & Enhancements
   ======================================== */

(function() {
    'use strict';

    // ========================================
    // Save Job Functionality
    // ========================================
    
    const initSaveJobButtons = () => {
        const saveButtons = document.querySelectorAll('.btn-save');
        
        saveButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                e.preventDefault();
                const icon = button.querySelector('i');
                button.classList.toggle('saved');
                
                if (button.classList.contains('saved')) {
                    icon.classList.remove('bi-bookmark');
                    icon.classList.add('bi-bookmark-fill');
                    showNotification('Job saved!', 'success');
                } else {
                    icon.classList.remove('bi-bookmark-fill');
                    icon.classList.add('bi-bookmark');
                    showNotification('Job removed from saved.', 'info');
                }
            });
        });
    };

    // ========================================
    // Apply Job Functionality
    // ========================================
    
    const initApplyButtons = () => {
        const applyButtons = document.querySelectorAll('.btn-apply');
        
        applyButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                e.preventDefault();
                showNotification('Redirecting to job details...', 'info');
                // In a real app, this would navigate to the job details page
            });
        });
    };

    // ========================================
    // Follow Company Functionality
    // ========================================
    
    const initFollowButtons = () => {
        const followButtons = document.querySelectorAll('.btn-follow');
        
        followButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                e.preventDefault();
                button.classList.toggle('followed');
                
                if (button.classList.contains('followed')) {
                    button.textContent = 'Following';
                    showNotification('Company added to your list!', 'success');
                } else {
                    button.textContent = 'Follow';
                    showNotification('Company removed from your list.', 'info');
                }
            });
        });
    };

    // ========================================
    // Search Form Interaction
    // ========================================
    
    const initSearchForm = () => {
        const jobTitleInput = document.getElementById('jobTitle');
        const locationInput = document.getElementById('location');
        const searchButton = document.querySelector('.btn-search');
        
        if (searchButton) {
            searchButton.addEventListener('click', (e) => {
                e.preventDefault();
                const jobTitle = jobTitleInput?.value?.trim() || '';
                const location = locationInput?.value?.trim() || '';
                
                if (jobTitle || location) {
                    showNotification(
                        `Searching for jobs: "${jobTitle}" in "${location || 'all locations'}"`,
                        'info'
                    );
                    // In a real app, this would submit the search
                } else {
                    showNotification('Please enter a job title or location', 'warning');
                }
            });
        }
    };

    // ========================================
    // Navigation Active State
    // ========================================
    
    const initNavigation = () => {
        const navLinks = document.querySelectorAll('.nav-link');
        
        navLinks.forEach(link => {
            link.addEventListener('click', function(e) {
                // Remove active class from all links
                navLinks.forEach(l => l.classList.remove('active'));
                // Add active class to clicked link
                this.classList.add('active');
            });
        });
    };

    // ========================================
    // Intersection Observer for Animations
    // ========================================
    
    const initScrollAnimations = () => {
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };
        
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                    observer.unobserve(entry.target);
                }
            });
        }, observerOptions);
        
        const animatedElements = document.querySelectorAll(
            '.job-card, .category-card, .company-card, .benefit-card, .resource-card'
        );
        
        animatedElements.forEach(el => {
            el.style.opacity = '0';
            el.style.transform = 'translateY(20px)';
            el.style.transition = 'opacity 0.6s ease-out, transform 0.6s ease-out';
            observer.observe(el);
        });
    };

    // ========================================
    // Notification System
    // ========================================
    
    const showNotification = (message, type = 'info') => {
        // Create notification element
        const notification = document.createElement('div');
        notification.className = `notification notification-${type}`;
        notification.textContent = message;
        notification.style.cssText = `
            position: fixed;
            bottom: 2rem;
            right: 2rem;
            padding: 1rem 1.5rem;
            background: ${getNotificationColor(type)};
            color: white;
            border-radius: 0.375rem;
            box-shadow: 0 10px 15px rgba(0, 0, 0, 0.1);
            font-weight: 500;
            font-size: 0.95rem;
            z-index: 9999;
            animation: slideInFromRight 0.3s ease-out;
            max-width: 400px;
        `;
        
        document.body.appendChild(notification);
        
        // Auto-remove after 3 seconds
        setTimeout(() => {
            notification.style.animation = 'slideOutToRight 0.3s ease-out forwards';
            setTimeout(() => {
                notification.remove();
            }, 300);
        }, 3000);
    };

    const getNotificationColor = (type) => {
        const colors = {
            success: '#16A34A',
            error: '#DC2626',
            warning: '#F59E0B',
            info: '#2563EB'
        };
        return colors[type] || colors.info;
    };

    // Add notification animations to stylesheet
    const addNotificationStyles = () => {
        const style = document.createElement('style');
        style.textContent = `
            @keyframes slideInFromRight {
                from {
                    transform: translateX(400px);
                    opacity: 0;
                }
                to {
                    transform: translateX(0);
                    opacity: 1;
                }
            }
            
            @keyframes slideOutToRight {
                from {
                    transform: translateX(0);
                    opacity: 1;
                }
                to {
                    transform: translateX(400px);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
    };

    // ========================================
    // Smooth Scroll Behavior
    // ========================================
    
    const initSmoothScroll = () => {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function(e) {
                const href = this.getAttribute('href');
                if (href === '#') return;
                
                e.preventDefault();
                const target = document.querySelector(href);
                
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    };

    // ========================================
    // Dark Mode Support (Optional)
    // ========================================
    
    const initDarkModeSupport = () => {
        // Check if system prefers dark mode
        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            // Could implement dark mode here
            // For now, we stick with light theme as per design
        }
    };

    // ========================================
    // Keyboard Navigation
    // ========================================
    
    const initKeyboardNavigation = () => {
        // Tab key focus visible styling is handled by CSS :focus-visible
        document.addEventListener('keydown', (e) => {
            // Escape key could close modals or notifications
            if (e.key === 'Escape') {
                const notifications = document.querySelectorAll('.notification');
                notifications.forEach(n => n.remove());
            }
        });
    };

    // ========================================
    // Scroll to Top Button (Optional Enhancement)
    // ========================================
    
    const initScrollToTop = () => {
        const scrollButton = document.createElement('button');
        scrollButton.innerHTML = '<i class="bi bi-arrow-up"></i>';
        scrollButton.className = 'scroll-to-top';
        scrollButton.title = 'Back to top';
        scrollButton.style.cssText = `
            position: fixed;
            bottom: 2rem;
            right: 2rem;
            width: 44px;
            height: 44px;
            background: #2563EB;
            color: white;
            border: none;
            border-radius: 50%;
            cursor: pointer;
            display: none;
            align-items: center;
            justify-content: center;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            transition: all 0.3s ease-out;
            font-size: 1.25rem;
            z-index: 999;
        `;
        
        document.body.appendChild(scrollButton);
        
        window.addEventListener('scroll', () => {
            if (window.scrollY > 300) {
                scrollButton.style.display = 'flex';
            } else {
                scrollButton.style.display = 'none';
            }
        });
        
        scrollButton.addEventListener('click', () => {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
        
        scrollButton.addEventListener('mouseenter', () => {
            scrollButton.style.transform = 'scale(1.1)';
        });
        
        scrollButton.addEventListener('mouseleave', () => {
            scrollButton.style.transform = 'scale(1)';
        });
    };

    // ========================================
    // Lazy Load Images (if needed)
    // ========================================
    
    const initLazyLoading = () => {
        if ('IntersectionObserver' in window) {
            const imageObserver = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        if (img.dataset.src) {
                            img.src = img.dataset.src;
                            img.removeAttribute('data-src');
                            imageObserver.unobserve(img);
                        }
                    }
                });
            });
            
            document.querySelectorAll('img[data-src]').forEach(img => {
                imageObserver.observe(img);
            });
        }
    };

    // ========================================
    // Initialize Everything on DOM Ready
    // ========================================
    
    document.addEventListener('DOMContentLoaded', () => {
        addNotificationStyles();
        initSaveJobButtons();
        initApplyButtons();
        initFollowButtons();
        initSearchForm();
        initNavigation();
        initScrollAnimations();
        initSmoothScroll();
        initDarkModeSupport();
        initKeyboardNavigation();
        initScrollToTop();
        initLazyLoading();
    });

    // Log initialization
    if (process.env.NODE_ENV !== 'production') {
        console.log('HuntCV interactions initialized');
    }

})();
