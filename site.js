// Utility functions for the application
const site = {
    // Initialize the application
    init: function() {
        this.setupEventListeners();
        this.checkBrowserSupport();
    },

    // Set up event listeners
    setupEventListeners: function() {
        document.addEventListener('DOMContentLoaded', () => {
            console.log('Application initialized');
        });
    },

    // Check browser support for required features
    checkBrowserSupport: function() {
        if (!window.fetch) {
            console.warn('Fetch API is not supported in this browser');
        }
    },

    // Handle API calls
    api: {
        // Make a GET request
        get: async function(url) {
            try {
                const response = await fetch(url);
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return await response.json();
            } catch (error) {
                console.error('Error fetching data:', error);
                throw error;
            }
        },

        // Make a POST request
        post: async function(url, data) {
            try {
                const response = await fetch(url, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(data)
                });
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return await response.json();
            } catch (error) {
                console.error('Error posting data:', error);
                throw error;
            }
        }
    },

    // Form handling utilities
    form: {
        // Serialize form data
        serialize: function(formElement) {
            const formData = new FormData(formElement);
            const data = {};
            for (let [key, value] of formData.entries()) {
                data[key] = value;
            }
            return data;
        },

        // Validate form
        validate: function(formElement) {
            const inputs = formElement.querySelectorAll('input[required], select[required], textarea[required]');
            let isValid = true;

            inputs.forEach(input => {
                if (!input.value.trim()) {
                    isValid = false;
                    input.classList.add('error');
                } else {
                    input.classList.remove('error');
                }
            });

            return isValid;
        }
    },

    // UI utilities
    ui: {
        // Show loading indicator
        showLoading: function(element) {
            element.classList.add('loading');
        },

        // Hide loading indicator
        hideLoading: function(element) {
            element.classList.remove('loading');
        },

        // Show error message
        showError: function(message, element) {
            const errorDiv = document.createElement('div');
            errorDiv.className = 'error-message';
            errorDiv.textContent = message;
            element.appendChild(errorDiv);
        },

        // Clear error messages
        clearErrors: function(element) {
            const errors = element.querySelectorAll('.error-message');
            errors.forEach(error => error.remove());
        }
    }
};

// Initialize the application when the script loads
site.init(); 