//Card Flip
//Add the event listener for each flip button access
const card_flip = document.querySelectorAll(".btn-flip");
card_flip.forEach(element => {
    element.addEventListener("click", function (e) {
        const card = e.target.parentElement.parentElement.parentElement;
        card.classList.toggle('is-flipped');
    });
});

$(".page-header ul a").on("click", function (e) {
    // 1
    e.preventDefault();
    // 2
    const href = $(this).attr("href");
    // 3
    $("html, body").animate({ scrollTop: $(href).offset().top }, 800);
});
