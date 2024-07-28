//Tỷ lệ
// function setProgressForAllCards() {
//   const cards = document.querySelectorAll('.product-card');

//   cards.forEach(card => {
//     const progressText = card.querySelector('.progress').innerText;
//     const progressValue = parseFloat(progressText);
//     const progressBar = card.querySelector('.scale-real');
//     progressBar.style.width = progressValue + '%';
//   });
// }

// window.onload = setProgressForAllCards;

//Heart

document.getElementById('btnICon').addEventListener('click', function () {
    var icons = this.querySelectorAll('i');
    icons.forEach(function (icon) {
        if (icon.style.display === 'none') {
            icon.style.display = 'inline';
        } else {
            icon.style.display = 'none';
        }
    });

    var projectId = this.getAttribute('data-project-id');
    fetch(`/User/ToggleLike?projectId=${projectId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    }).then(response => {
        if (response.ok) {
            console.log('Like toggled successfully.');
        } else {
            console.error('Failed to toggle like.');
        }
    }).catch(error => {
        console.error('Error:', error);
    });
});

// CHuyển giữa 2 content
document.addEventListener("DOMContentLoaded", function () {
    const btnContent = document.querySelector('.btnContent');
    const btnSup = document.querySelector('.btnSup');
    const textContent = document.querySelector('.Content');
    const supportList = document.querySelector('.supportList');

    textContent.classList.add('active');

    btnContent.addEventListener('click', function () {
        btnContent.classList.add('active');
        btnSup.classList.remove('active');
        textContent.classList.add('active');
        supportList.classList.remove('active');
    });

    btnSup.addEventListener('click', function () {
        supportList.classList.add('active');
        textContent.classList.remove('active');
        btnSup.classList.add('active');
        btnContent.classList.remove('active');
    });
});

//btnInvite
const inviteBox = document.getElementById('inviteBox');
const overlay = document.getElementById('overlay');
const inviteButton = document.querySelector('.invite-btn');

function toggleInviteBox() {
    const isShown = inviteBox.classList.contains('show');
    inviteBox.classList.toggle('show', !isShown);
    overlay.classList.toggle('show', !isShown);
}
inviteButton.addEventListener('click', toggleInviteBox);

overlay.addEventListener('click', function () {
    inviteBox.classList.remove('show');
    overlay.classList.remove('show');
});

//ProductImg
// let currentSlide = 0;
// const slidesToShow = 5;
// const productsImg = document.getElementById('productsImg');
// const totalSlides = productsImg.children.length;

// function updateSliderPosition() {
//     const offset = -currentSlide * 150;
//     productsImg.style.transform = `translateX(${offset}px)`;
// }

// function nextSlide() {
//     if (currentSlide < totalSlides - slidesToShow) {
//         currentSlide++;
//     }else{
//       currentSlide = 0;
//     }
//       updateSliderPosition();
// }

// function prevSlide() {
//     if (currentSlide > 0) {
//         currentSlide--;
//     }else{
//       currentSlide = totalSlides - slidesToShow;
//     }
//     updateSliderPosition();
// }

//Chọn ảnh
// function clickme(smallImg){
//   let fullImg =  document.getElementById("imgBox");
//   fullImg.src = smallImg.src;
// }

var swiper = new Swiper('.mySwiper', {
    loop: true,
    spaceBetween: 10,
    slidesPerView: 3,
    freeMode: true,
});
var swiper2 = new Swiper('.mySwiper2', {
    loop: true,
    spaceBetween: 10,
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev',
    },
    thumbs: {
        swiper: swiper,
    },
});

