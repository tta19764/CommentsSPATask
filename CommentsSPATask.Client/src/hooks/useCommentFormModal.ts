import { type SubmitEvent, useEffect, useState } from "react";
import { getApiErrorFromMutationError, getApiErrorText } from "../api/apiError";
import { useCreateCaptchaMutation } from "../api/captchasApi";
import {
  useCreateCommentMutation,
  useCreateReplyMutation,
} from "../api/commentsApi";
import { buildCommentFormData } from "../builders/buildCommentFormData";
import { buildApiUrl } from "../utils/apiUrl";

const MAX_TEXT_FILE_SIZE_BYTES = 100 * 1024;

type UseCommentFormModalOptions = {
  onClose: () => void;
  parentId?: string;
};

export function useCommentFormModal({
  onClose,
  parentId,
}: UseCommentFormModalOptions) {
  const [userName, setUserName] = useState("");
  const [email, setEmail] = useState("");
  const [homePage, setHomePage] = useState("");
  const [text, setText] = useState("");
  const [captchaInput, setCaptchaInput] = useState("");
  const [attachment, setAttachment] = useState<File | undefined>();
  const [attachmentError, setAttachmentError] = useState<string | null>(null);
  const [submitError, setSubmitError] = useState<string | null>(null);

  const [createCaptcha, { data: captchaResponse, isLoading: isCaptchaLoading }] =
    useCreateCaptchaMutation();
  const [createComment, { isLoading: isCreatingComment }] =
    useCreateCommentMutation();
  const [createReply, { isLoading: isCreatingReply }] = useCreateReplyMutation();

  useEffect(() => {
    void createCaptcha();
  }, [createCaptcha]);

  const captcha = captchaResponse?.data;
  const captchaImageUrl = captcha?.imageUrl ? buildApiUrl(captcha.imageUrl) : null;
  const isSubmitting = isCreatingComment || isCreatingReply;

  const refreshCaptcha = () => {
    setCaptchaInput("");
    void createCaptcha();
  };

  const handleSubmit = async (event: SubmitEvent<HTMLFormElement>) => {
    event.preventDefault();
    setSubmitError(null);

    if (attachmentError || !captcha?.captchaId) {
      setSubmitError(attachmentError ?? "Captcha is not ready yet.");
      return;
    }

    const formData = buildCommentFormData({
      userName,
      email,
      homePage,
      text,
      captchaId: captcha.captchaId,
      captchaInput,
      attachment,
    });

    const result = parentId
      ? await createReply({ parentId, formData })
      : await createComment(formData);

    if ("error" in result || result.data?.error) {
      const apiError =
        "error" in result
          ? getApiErrorFromMutationError(result.error)
          : result.data.error;
      setSubmitError(getApiErrorText(apiError) ?? "Could not save the comment.");
      refreshCaptcha();
      return;
    }

    onClose();
  };

  const handleAttachmentChange = (file: File | undefined) => {
    setAttachmentError(null);
    setAttachment(file);

    if (!file) return;

    const isTextFile =
      file.type === "text/plain" || file.name.toLowerCase().endsWith(".txt");

    if (isTextFile && file.size > MAX_TEXT_FILE_SIZE_BYTES) {
      setAttachment(undefined);
      setAttachmentError("Text attachment must be 100 KB or smaller.");
    }
  };

  return {
    attachmentError,
    captchaImageUrl,
    captchaInput,
    email,
    handleAttachmentChange,
    handleSubmit,
    homePage,
    isCaptchaLoading,
    isSubmitting,
    refreshCaptcha,
    setCaptchaInput,
    setEmail,
    setHomePage,
    setText,
    setUserName,
    submitError,
    text,
    userName,
  };
}
