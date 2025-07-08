// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// 共通で利用されるメソッドをモジュール構文で定義
export class CommonUtils {
  // CSRFトークンを取得する関数
  static getCsrfToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')
      .value;
  }

  // HTMLエスケープ関数
  static escapeHtml(text) {
    const div = document.createElement("div");
    div.textContent = text;
    return div.innerHTML;
  }

  // メッセージを表示する関数
  static showMessage(message, type, duration = 5000) {
    const messageDiv = document.getElementById("message");
    if (messageDiv) {
      messageDiv.textContent = message;
      messageDiv.className = `alert alert-${type}`;
      messageDiv.style.display = "block";

      // 指定時間後にメッセージを非表示
      setTimeout(() => {
        messageDiv.style.display = "none";
      }, duration);
    }
  }

  // ローディング表示を制御する関数
  static showLoading(loadingElementId = "loadingMessage") {
    const loadingElement = document.getElementById(loadingElementId);
    if (loadingElement) {
      loadingElement.style.display = "block";
    }
  }

  static hideLoading(loadingElementId = "loadingMessage") {
    const loadingElement = document.getElementById(loadingElementId);
    if (loadingElement) {
      loadingElement.style.display = "none";
    }
  }

  // エラーメッセージを表示する関数
  static showError(message, errorElementId = "errorMessage") {
    const errorElement = document.getElementById(errorElementId);
    if (errorElement) {
      errorElement.textContent = message;
      errorElement.style.display = "block";
    }
  }

  static hideError(errorElementId = "errorMessage") {
    const errorElement = document.getElementById(errorElementId);
    if (errorElement) {
      errorElement.style.display = "none";
    }
  }

  // 確認ダイアログを表示する関数
  static async confirm(message) {
    return confirm(message);
  }

  // アラートを表示する関数
  static alert(message) {
    alert(message);
  }
}

// API通信の共通クラス
export class ApiClient {
  static async get(url) {
    try {
      const response = await fetch(url, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
      });
      return await response.json();
    } catch (error) {
      throw new Error("ネットワークエラーが発生しました: " + error.message);
    }
  }

  static async post(url, data) {
    try {
      const response = await fetch(url, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          RequestVerificationToken: CommonUtils.getCsrfToken(),
        },
        body: JSON.stringify(data),
      });
      return await response.json();
    } catch (error) {
      throw new Error("ネットワークエラーが発生しました: " + error.message);
    }
  }

  static async delete(url) {
    try {
      const response = await fetch(url, {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json",
          RequestVerificationToken: CommonUtils.getCsrfToken(),
        },
      });
      return await response.json();
    } catch (error) {
      throw new Error("ネットワークエラーが発生しました: " + error.message);
    }
  }
}
