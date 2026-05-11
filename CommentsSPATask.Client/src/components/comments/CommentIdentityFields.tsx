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
  return (
    <>
      <div className="col-md-6">
        <label className="form-label" htmlFor="comment-user-name">User name</label>
        <input
          className="form-control"
          id="comment-user-name"
          onChange={(event) => setUserName(event.target.value)}
          pattern="[A-Za-z0-9]+"
          required
          value={userName}
        />
      </div>

      <div className="col-md-6">
        <label className="form-label" htmlFor="comment-email">E-mail</label>
        <input
          className="form-control"
          id="comment-email"
          onChange={(event) => setEmail(event.target.value)}
          required
          type="email"
          value={email}
        />
      </div>

      <div className="col-12">
        <label className="form-label" htmlFor="comment-home-page">Home page</label>
        <input
          className="form-control"
          id="comment-home-page"
          onChange={(event) => setHomePage(event.target.value)}
          type="url"
          value={homePage}
        />
      </div>
    </>
  );
}

export default CommentIdentityFields;
