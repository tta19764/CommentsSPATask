import { useEffect, useRef } from "react";

type CommentIdentityFieldsProps = {
  email: string;
  homePage: string;
  setEmail: (value: string) => void;
  setHomePage: (value: string) => void;
  setUserName: (value: string) => void;
  userName: string;
};

function CommentIdentityFields({
  email,
  homePage,
  setEmail,
  setHomePage,
  setUserName,
  userName,
}: CommentIdentityFieldsProps) {
  const userNameRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    const message =
      userName && !/^[A-Za-z0-9]+$/.test(userName)
        ? "Use only Latin letters and digits. Spaces and symbols are not allowed."
        : "";

    userNameRef.current?.setCustomValidity(message);
  }, [userName]);

  return (
    <>
      <div className="col-md-6">
        <label className="form-label" htmlFor="comment-user-name">User name</label>
        <input
          className="form-control"
          aria-describedby="comment-user-name-help"
          id="comment-user-name"
          maxLength={50}
          onChange={(event) => setUserName(event.target.value)}
          pattern="[A-Za-z0-9]+"
          ref={userNameRef}
          required
          title="Use only Latin letters and digits. Spaces and symbols are not allowed."
          value={userName}
        />
        <div className="form-text" id="comment-user-name-help">
          Required. Latin letters and digits only, up to 50 characters.
        </div>
      </div>

      <div className="col-md-6">
        <label className="form-label" htmlFor="comment-email">E-mail</label>
        <input
          className="form-control"
          aria-describedby="comment-email-help"
          id="comment-email"
          maxLength={100}
          onChange={(event) => setEmail(event.target.value)}
          required
          title="Enter a valid email address, for example user@example.com."
          type="email"
          value={email}
        />
        <div className="form-text" id="comment-email-help">
          Required. Use a valid email format, up to 100 characters.
        </div>
      </div>

      <div className="col-12">
        <label className="form-label" htmlFor="comment-home-page">Home page</label>
        <input
          className="form-control"
          aria-describedby="comment-home-page-help"
          id="comment-home-page"
          maxLength={250}
          onChange={(event) => setHomePage(event.target.value)}
          title="Optional. Use an absolute URL beginning with http:// or https://."
          type="url"
          value={homePage}
        />
        <div className="form-text" id="comment-home-page-help">
          Optional. Must start with http:// or https:// when provided.
        </div>
      </div>
    </>
  );
}

export default CommentIdentityFields;
