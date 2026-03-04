$(function () {
    //取得目前的網址路徑並轉小寫
    let currentPath = window.location.pathname.toLowerCase();

    //尋找側邊欄所有的連結
    $('.staff-sidebar .nav-link').each(function () {
        let href = $(this).attr('href');

        // 確保 href 屬性存在才進行比對
        if (href) {
            href = href.toLowerCase();

            // 如果目前的網址開頭包含這個連結的路徑 
            if (href !== "/" && currentPath.startsWith(href)) {
                // 找到相符的，直接加上 active class！
                $(this).addClass('active');
            }
        }
    });
});