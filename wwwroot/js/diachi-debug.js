// Debug script for address management
console.log('DiaChi Debug Script Loaded');

// Simple test functions
function testOpenCreateModal() {
    console.log('Testing create modal...');
    try {
        const modal = new bootstrap.Modal(document.getElementById('createAddressModal'));
        modal.show();
        console.log('Create modal opened successfully');
    } catch (error) {
        console.error('Error opening create modal:', error);
    }
}

function testOpenEditModal(addressId) {
    console.log('Testing edit modal for ID:', addressId);
    try {
        const modal = new bootstrap.Modal(document.getElementById('editAddressModal'));
        modal.show();
        console.log('Edit modal opened successfully');
    } catch (error) {
        console.error('Error opening edit modal:', error);
    }
}

function testDeleteAddress(addressId) {
    console.log('Testing delete for ID:', addressId);
    if (confirm('Test delete - Are you sure?')) {
        console.log('User confirmed delete for ID:', addressId);
        alert('This is a test delete function');
    }
}

function testSetDefault(addressId) {
    console.log('Testing set default for ID:', addressId);
    if (confirm('Test set default - Are you sure?')) {
        console.log('User confirmed set default for ID:', addressId);
        alert('This is a test set default function');
    }
}

// Check if required elements exist
function checkElements() {
    console.log('Checking required elements...');
    
    const createModal = document.getElementById('createAddressModal');
    const editModal = document.getElementById('editAddressModal');
    
    console.log('Create modal found:', !!createModal);
    console.log('Edit modal found:', !!editModal);
    
    // Check if Bootstrap is loaded
    console.log('Bootstrap available:', typeof bootstrap !== 'undefined');
    
    // Check antiforgery token
    const token = document.querySelector('input[name="__RequestVerificationToken"]');
    console.log('Antiforgery token found:', !!token);
    
    return {
        createModal: !!createModal,
        editModal: !!editModal,
        bootstrap: typeof bootstrap !== 'undefined',
        token: !!token
    };
}

// Auto-run check when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    console.log('DOM loaded, running element check...');
    const results = checkElements();
    console.log('Element check results:', results);
});
