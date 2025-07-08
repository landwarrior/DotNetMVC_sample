// ユーザー作成画面専用のJavaScript
import { ApiClient, CommonUtils } from "./site.js";

// ユーザー作成管理クラス
class UserCreateManager {
  constructor() {
    this.initializeEventListeners();
  }

  // イベントリスナーの初期化
  initializeEventListeners() {
    const createUserForm = document.getElementById("createUserForm");
    if (createUserForm) {
      createUserForm.addEventListener("submit", (e) => {
        e.preventDefault();
        this.handleFormSubmit();
      });
    }
  }

  // フォーム送信の処理
  async handleFormSubmit() {
    const userName = document.getElementById("userName").value;
    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;

    // 基本的なバリデーション
    if (!userName || !email || !password) {
      CommonUtils.showMessage("すべての項目を入力してください。", "danger");
      return;
    }

    try {
      const result = await ApiClient.post("/api/users", {
        userName: userName,
        email: email,
        passwordHash: password,
      });

      if (result.success) {
        CommonUtils.showMessage("ユーザーが正常に作成されました。", "success");
        // フォームをクリア
        document.getElementById("createUserForm").reset();
        // 3秒後に一覧ページに戻る
        setTimeout(() => {
          window.location.href = "/user";
        }, 3000);
      } else {
        CommonUtils.showMessage(
          "作成に失敗しました: " + result.message,
          "danger"
        );
      }
    } catch (error) {
      CommonUtils.showMessage(
        "ネットワークエラーが発生しました: " + error.message,
        "danger"
      );
    }
  }
}

// ページ読み込み時の初期化
document.addEventListener("DOMContentLoaded", function () {
  new UserCreateManager();
});
