// scan.js — Image preview and form UX for Upload page
document.addEventListener('DOMContentLoaded', () => {
    const input = document.getElementById('imageInput');
    const previewContainer = document.getElementById('imagePreview');
    const previewImg = document.getElementById('previewImg');

    if (input && previewContainer && previewImg) {
        input.addEventListener('change', () => {
            const file = input.files[0];
            if (!file) {
                previewContainer.style.display = 'none';
                return;
            }

            const validTypes = ['image/jpeg', 'image/png', 'image/webp', 'image/heic'];
            if (!validTypes.includes(file.type)) {
                alert('Please select a JPEG, PNG, WebP, or HEIC image.');
                input.value = '';
                previewContainer.style.display = 'none';
                return;
            }

            if (file.size > 10 * 1024 * 1024) {
                alert('Image must be under 10 MB.');
                input.value = '';
                previewContainer.style.display = 'none';
                return;
            }

            const reader = new FileReader();
            reader.onload = (e) => {
                previewImg.src = e.target.result;
                previewContainer.style.display = 'block';
            };
            reader.readAsDataURL(file);
        });
    }

    // Loading state for submit buttons
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', () => {
            const btn = form.querySelector('button[type="submit"]');
            if (btn && !btn.disabled) {
                btn.disabled = true;
                btn.dataset.originalText = btn.innerHTML;
                btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Processing…';
            }
        });
    });
});
