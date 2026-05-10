const ALLOWED_TAGS = new Set(["A", "CODE", "I", "STRONG"]);
const ALLOWED_ATTRIBUTES = new Set(["href", "title"]);

export function sanitizeCommentHtml(value: string) {
  const template = document.createElement("template");
  template.innerHTML = value;

  sanitizeChildren(template.content);

  return template.innerHTML;
}

function sanitizeChildren(parent: ParentNode) {
  for (const child of Array.from(parent.childNodes)) {
    if (child.nodeType === Node.TEXT_NODE) {
      continue;
    }

    if (child.nodeType !== Node.ELEMENT_NODE) {
      child.remove();
      continue;
    }

    const element = child as HTMLElement;

    if (!ALLOWED_TAGS.has(element.tagName)) {
      element.replaceWith(document.createTextNode(element.textContent ?? ""));
      continue;
    }

    for (const attribute of Array.from(element.attributes)) {
      const name = attribute.name.toLowerCase();

      if (element.tagName !== "A" || !ALLOWED_ATTRIBUTES.has(name)) {
        element.removeAttribute(attribute.name);
      }
    }

    if (element.tagName === "A") {
      const href = element.getAttribute("href")?.trim();

      if (!href || !isSafeHref(href)) {
        element.removeAttribute("href");
      }

      element.setAttribute("rel", "noreferrer");
      element.setAttribute("target", "_blank");
    }

    sanitizeChildren(element);
  }
}

function isSafeHref(href: string) {
  if (href.startsWith("//")) {
    return false;
  }

  try {
    const url = new URL(href, window.location.origin);

    if (url.protocol !== "http:" && url.protocol !== "https:") {
      return false;
    }

    const isRelativeUrl = !/^[a-z][a-z0-9+.-]*:/i.test(href);

    return isRelativeUrl || url.protocol === "http:" || url.protocol === "https:";
  } catch {
    return false;
  }
}
