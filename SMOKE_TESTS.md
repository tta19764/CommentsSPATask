# Smoke Tests

The table below is a smoke checklist for the full task.  
Each row can be used manually by QA or as a release-readiness checklist before submission.

| Done | ID | Area | Test case | Preconditions | Steps | Expected result |
|------|---|---|---|---|---|---|
| [+]  | ST-001 | Startup | Docker stack starts successfully | Docker is installed | 1. Run `docker compose up --build`<br>2. Wait for all containers to start | Client, API, and SQL Server containers are running without crash loops |
| [+]  | ST-002 | Startup | SQL Server healthcheck passes before API startup | Docker stack is starting | 1. Review container logs<br>2. Check container health state | SQL Server reaches `healthy` state before API accepts requests |
| [+]  | ST-003 | Startup | API root redirects to Swagger in development | API is running in Development | 1. Open API root URL | Root URL redirects to `/swagger` |
| [+]  | ST-004 | Startup | Swagger UI loads | API is running | 1. Open `/swagger` | Swagger page loads and documents the API |
| [+]  | ST-005 | Startup | Migrations apply on startup | Empty or existing SQL Server database | 1. Start API<br>2. Review logs | Database schema is created or updated without manual intervention |
| [+]  | ST-006 | Main page | Main page loads | Full stack is running | 1. Open client URL | Comments page loads without runtime errors |
| [+]  | ST-007 | Main page | Root comments are shown in a table | At least one root comment exists | 1. Open main page | Root comments are displayed in tabular form |
| [+]  | ST-008 | Form | Add comment form opens | Client is running | 1. Click add-comment action | Modal/form opens and shows all required fields |
| [+]  | ST-009 | CAPTCHA | CAPTCHA image is displayed | Comment form is open | 1. Open add comment form | CAPTCHA image and input field are visible |
| [+]  | ST-010 | CAPTCHA | CAPTCHA refresh works | Comment form is open | 1. Click CAPTCHA refresh | New CAPTCHA is generated and displayed |
| [+]  | ST-011 | CAPTCHA | Valid CAPTCHA allows posting | Valid form data is prepared | 1. Fill valid form<br>2. Enter valid CAPTCHA<br>3. Submit | Comment is created successfully |
| [+]  | ST-012 | CAPTCHA | Invalid CAPTCHA blocks posting | Valid form data except CAPTCHA | 1. Fill valid form<br>2. Enter invalid CAPTCHA<br>3. Submit | Comment is rejected and an error is shown |
| [+]  | ST-013 | CAPTCHA | Expired CAPTCHA is rejected | CAPTCHA exists and has expired | 1. Wait until CAPTCHA expires<br>2. Submit form | Comment is rejected with captcha-expired error |
| [+]  | ST-014 | Validation | `User Name` is required | Comment form is open | 1. Leave `User Name` empty<br>2. Submit | Client and/or server validation blocks submission |
| [+]  | ST-015 | Validation | `User Name` accepts only latin letters and digits | Comment form is open | 1. Enter invalid characters or Cyrillic<br>2. Submit | Validation error is shown |
| [+]  | ST-016 | Validation | `E-mail` is required | Comment form is open | 1. Leave `E-mail` empty<br>2. Submit | Submission is blocked |
| [+]  | ST-017 | Validation | `E-mail` format is validated | Comment form is open | 1. Enter invalid email format<br>2. Submit | Validation error is shown |
| [+]  | ST-018 | Validation | `Home page` is optional | Comment form is open | 1. Leave `Home page` empty<br>2. Submit valid form | Comment is created successfully |
| [+]  | ST-019 | Validation | `Home page` format is validated when provided | Comment form is open | 1. Enter invalid URL<br>2. Submit | Validation error is shown |
| [+]  | ST-020 | Validation | `Text` is required | Comment form is open | 1. Leave `Text` empty<br>2. Submit | Submission is blocked |
| [+]  | ST-021 | HTML | Allowed tags are accepted | Comment form is open | 1. Enter text with `<a>`, `<code>`, `<i>`, `<strong>`<br>2. Submit | Comment is saved and rendered with allowed formatting |
| [+]  | ST-022 | HTML | Unsupported tags are rejected | Comment form is open | 1. Enter text with `<script>` or other unsupported tags<br>2. Submit | Submission is rejected or sanitized according to policy; no unsafe output remains |
| [+]  | ST-023 | HTML | Invalid markup is rejected | Comment form is open | 1. Enter unclosed or invalid nested tags<br>2. Submit | Validation rejects the message |
| [+]  | ST-024 | HTML | Unsafe link protocols are rejected | Comment form is open | 1. Enter `<a href="javascript:...">`<br>2. Submit | Submission is rejected |
| [+]  | ST-025 | Preview | Message preview works without page reload | Comment form is open | 1. Enter text with allowed tags<br>2. Open preview | Preview displays rendered text without full page reload |
| [+]  | ST-026 | Toolbar | HTML tag buttons insert markup | Comment form is open | 1. Click `[i]`, `[strong]`, `[code]`, `[a]` buttons | Corresponding tag markup is inserted into the editor |
| [+]  | ST-027 | Comments | Root comment can be created | Valid form data exists | 1. Submit a root comment | Comment appears as a root entry |
| [+]  | ST-028 | Comments | Reply can be created | At least one root comment exists | 1. Open reply form for a comment<br>2. Submit valid reply | Reply is linked to the correct parent |
| [+]  | ST-029 | Comments | Nested replies are displayed correctly | Existing comment thread with multiple levels | 1. Create replies on replies | Thread is rendered recursively without layout breakage |
| [ ]  | ST-030 | Sorting | Sort by `User Name` ascending | At least three root comments with different names | 1. Select sort by `User Name` ascending | Table order matches ascending user names |
| [ ]  | ST-031 | Sorting | Sort by `User Name` descending | Same as above | 1. Select sort by `User Name` descending | Table order matches descending user names |
| [ ]  | ST-032 | Sorting | Sort by `E-mail` ascending/descending | At least three root comments with different emails | 1. Select `E-mail` ascending<br>2. Select `E-mail` descending | Order matches selected direction |
| [ ]  | ST-033 | Sorting | Sort by created date ascending/descending | At least three root comments created at different times | 1. Select date ascending<br>2. Select date descending | Order matches selected direction |
| [ ]  | ST-034 | Sorting | Default sort is LIFO | Multiple root comments exist | 1. Open page without changing sort | Newest root comment is displayed first |
| [ ]  | ST-035 | Paging | Root comments are paged by 25 | More than 25 root comments exist | 1. Open first page | Only 25 root comments are shown |
| [ ]  | ST-036 | Paging | Pagination navigation works | More than 25 root comments exist | 1. Open page 2<br>2. Return to page 1 | Page navigation works and state updates correctly |
| [ ]  | ST-037 | Attachments | Valid TXT file can be uploaded | Comment form is open | 1. Attach `.txt` file under 100 KB<br>2. Submit | Comment is created and file is attached |
| [ ]  | ST-038 | Attachments | TXT larger than 100 KB is rejected | Comment form is open | 1. Attach `.txt` file larger than 100 KB<br>2. Submit | Submission is rejected |
| [ ]  | ST-039 | Attachments | Valid JPG/PNG/GIF can be uploaded | Comment form is open | 1. Attach supported image format<br>2. Submit | Comment is created and attachment is stored |
| [ ]  | ST-040 | Attachments | Unsupported file type is rejected | Comment form is open | 1. Attach `.zip`, `.pdf`, or other unsupported file<br>2. Submit | Submission is rejected |
| [ ]  | ST-041 | Attachments | Large image is resized before saving | Comment form is open with image larger than `320x240` | 1. Attach oversized image<br>2. Submit<br>3. Open uploaded file | Saved image dimensions do not exceed `320x240` and preserve aspect ratio |
| [ ]  | ST-042 | Attachments | Small image is not enlarged | Comment form is open with image smaller than limits | 1. Attach small image<br>2. Submit<br>3. Open uploaded file | Image remains valid and is not enlarged |
| [ ]  | ST-043 | Attachments | Text file is served with readable encoding | Existing comment with UTF-8 `.txt` attachment | 1. Open attachment URL | Text is shown without encoding corruption |
| [ ]  | ST-044 | Attachments | Attachment preview opens with visual effect | Existing comment with attachment | 1. Click attachment preview | Attachment opens in modal/lightbox-style UI |
| [ ]  | ST-045 | Security | SQL injection payloads do not break persistence | Comment form is open | 1. Enter SQL injection strings in fields<br>2. Submit | Application treats input as data; DB remains intact |
| [ ]  | ST-046 | Security | XSS payload does not execute in rendered comments | Comment form is open | 1. Submit comment with XSS payload<br>2. Open list and thread views | Script does not execute |
| [ ]  | ST-047 | Security | Server-side validation rejects invalid direct API requests | API is reachable | 1. Submit malformed payload directly to API | API returns validation error and persists nothing |
| [ ]  | ST-048 | Realtime | New root comment refreshes other clients | Two browser sessions are open | 1. Create comment in session A | Session B refreshes comment data via SignalR |
| [ ]  | ST-049 | Realtime | New reply refreshes open thread in other client | Two browser sessions are open on same thread | 1. Create reply in session A | Session B refreshes thread data |
| [ ]  | ST-050 | Files | Uploaded attachment URL is reachable | Existing attachment exists | 1. Open `/uploads/{storedFileName}` | File is returned successfully |
| [ ]  | ST-051 | Persistence | Comments survive container restart | Existing comments and attachments exist | 1. Restart Docker stack<br>2. Open application | Comments remain in database and files remain available |
| [ ]  | ST-052 | Persistence | CAPTCHA creation still works after restart | Docker stack restarted | 1. Open comment form | New CAPTCHA is generated successfully |
| [ ]  | ST-053 | Edge case | Reply to non-existent parent is rejected | API is reachable | 1. Submit reply with random parent id | API returns parent-not-found error |
| [ ]  | ST-054 | Edge case | Used CAPTCHA cannot be reused | One valid submission already consumed a captcha | 1. Reuse same captcha id | API rejects request |
| [ ]  | ST-055 | Edge case | Thread with many replies still renders | Existing thread with many replies | 1. Open thread | UI renders all replies without crashing |
| [ ]  | ST-056 | Docs | Root README describes the project | Repository is available | 1. Open `README.md` | README explains architecture, features, run flow, and testing |
| [ ]  | ST-057 | Docs | Smoke checklist exists and is readable | Repository is available | 1. Open `SMOKE_TESTS.md` | Checklist is present as a table with checkboxes |
| [ ]  | ST-058 | Delivery | Docker stack is self-contained for local verification | Clean machine with Docker | 1. Clone repo<br>2. Run Docker Compose | Application starts without manual environment patching beyond documented values |
