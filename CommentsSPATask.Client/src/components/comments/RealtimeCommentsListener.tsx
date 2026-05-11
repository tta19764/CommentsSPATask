import { HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr";
import { useEffect } from "react";
import { useDispatch } from "react-redux";
import { commentsApi } from "../../api/commentsApi";
import { buildApiUrl } from "../../utils/apiUrl";
const HUB_URL = buildApiUrl(import.meta.env.VITE_APP_HUB_ENDPOINT.trim() || "hubs/comments");

function RealtimeCommentsListener() {
  const dispatch = useDispatch();

  useEffect(() => {
    let isDisposed = false;

    const connection = new HubConnectionBuilder()
      .withUrl(HUB_URL, {
        withCredentials: false,
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    const refreshComments = () => {
      dispatch(commentsApi.util.invalidateTags(["Comments"]));
    };

    connection.on("CommentCreated", refreshComments);
    connection.on("AttachmentCreated", refreshComments);

    void connection.start().catch((error: unknown) => {
      if (isDisposed || isAbortError(error)) {
        return;
      }

      console.warn("SignalR comments connection failed.", error);
    });

    return () => {
      isDisposed = true;
      connection.off("CommentCreated", refreshComments);
      connection.off("AttachmentCreated", refreshComments);

      if (connection.state !== HubConnectionState.Disconnected) {
        void connection.stop();
      }
    };
  }, [dispatch]);

  return null;
}

function isAbortError(error: unknown) {
  return Boolean(
    error &&
      typeof error === "object" &&
      "name" in error &&
      (error as { name?: string }).name === "AbortError"
  );
}

export default RealtimeCommentsListener;
