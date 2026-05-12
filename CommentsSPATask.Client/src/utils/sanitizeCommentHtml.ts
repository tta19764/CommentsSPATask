const ALLOWED_TAGS = new Set(["A", "CODE", "I", "STRONG"]);
const ALLOWED_ATTRIBUTES = new Set(["href", "title"]);

export function validateCommentHtml(value: string) {
  const template = document.createElement("template");
  template.innerHTML = value;
  const errors = validateClosingTags(value);

  validateChildren(template.content, errors);

  return errors;
}

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

function validateChildren(parent: ParentNode, errors: string[]) {
  for (const child of Array.from(parent.childNodes)) {
    if (child.nodeType === Node.TEXT_NODE) {
      continue;
    }

    if (child.nodeType !== Node.ELEMENT_NODE) {
      errors.push("Only text and allowed HTML tags are supported.");
      continue;
    }

    const element = child as HTMLElement;
    const tagName = element.tagName.toLowerCase();

    if (!ALLOWED_TAGS.has(element.tagName)) {
      errors.push(`<${tagName}> is not allowed.`);
      continue;
    }

    validateAttributes(element, errors);
    validateChildren(element, errors);

    if (!element.textContent?.trim()) {
      errors.push(`<${tagName}> must contain text.`);
    }
  }
}

function validateClosingTags(value: string) {
  const errors: string[] = [];
  const stack: string[] = [];
  const tagRegex = /<\/?([a-z][a-z0-9]*)(?:\s[^<>]*)?>/gi;

  for (const match of value.matchAll(tagRegex)) {
    const fullTag = match[0];
    const tagName = match[1].toLowerCase();

    if (!ALLOWED_TAGS.has(tagName.toUpperCase())) {
      continue;
    }

    if (fullTag.endsWith("/>")) {
      errors.push(`<${tagName}> must use an opening and closing tag.`);
      continue;
    }

    if (fullTag.startsWith("</")) {
      const expected = stack.pop();

      if (expected !== tagName) {
        errors.push(`Closing tag </${tagName}> does not match the opened tag.`);
      }

      continue;
    }

    stack.push(tagName);
  }

  for (const tagName of stack.reverse()) {
    errors.push(`Add closing tag </${tagName}>.`);
  }

  return errors;
}

function validateAttributes(element: HTMLElement, errors: string[]) {
  const tagName = element.tagName.toLowerCase();

  for (const attribute of Array.from(element.attributes)) {
    const name = attribute.name.toLowerCase();

    if (element.tagName !== "A" || !ALLOWED_ATTRIBUTES.has(name)) {
      errors.push(`<${tagName}> cannot use "${attribute.name}".`);
      continue;
    }

    if (name === "href" && !isSafeHref(attribute.value.trim())) {
      errors.push("Link href must be a safe relative, http, or https URL.");
    }
  }
}

function isSafeHref(href: string) {
  if (!href) {
    return false;
  }

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
