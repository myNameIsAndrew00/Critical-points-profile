﻿
export function HtmlToElement(html) {
    var template = document.createElement('template');
    template.innerHTML = html.trim();

    return template.content.firstChild;
}
 