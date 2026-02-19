function Preview_image(){

    const input = document.querySelector('input');
    const preview = document.querySelector('.preview');

    // input.style.opacity = 0;

    input.addEventListener('change', updateImageDisplay);

    function updateImageDisplay() {
        while(preview.firstChild) {
            preview.removeChild(preview.firstChild);
        }

        const file_imgs = input.files;
        if(file_imgs.length === 0) {
            const para = document.createElement('p');
            para.textContent = 'No file currently selected for upload';
            preview.appendChild(para);
        } else {
            const img = file_imgs[0];             
            const para = document.createElement('p');
            
            if(validFileType(img)) {
                para.textContent = `File name ${img.name}, file size ${returnFileSize(img.size)}.`;
                const image = document.createElement('img');
                image.src = URL.createObjectURL(img);
                
                preview.appendChild(image);
                
            } else {
                para.textContent = `File name ${img.name}: Not a valid file type. Update your selection.`;
            }
            
            preview.appendChild(para);
        }
    }

// https://developer.mozilla.org/en-US/docs/Web/Media/Formats/Image_types
    const fileTypes = [
        'image/apng',
        'image/bmp',
        'image/gif',
        'image/jpeg',
        'image/pjpeg',
        'image/png',
        'image/svg+xml',
        'image/tiff',
        'image/webp',
        `image/x-icon`
    ];

    function validFileType(file) {
        console.log(file.type);
      return fileTypes.includes(file.type);
    }

    function returnFileSize(number) {
      if(number < 1024) {
        return number + 'bytes';
      } else if(number > 1024 && number < 1048576) {
        return (number/1024).toFixed(1) + 'KB';
      } else if(number > 1048576) {
        return (number/1048576).toFixed(1) + 'MB';
      }
    }
}

Preview_image()