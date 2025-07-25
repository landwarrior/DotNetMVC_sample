// ユーザー画面専用のJavaScript
import { ApiClient, CommonUtils } from "./site.js";

// ユーザー管理クラス
class UserManager {
  constructor() {
    this.initializeEventListeners();
    this.loadUsers();
    this.handleMessages();
  }

  // イベントリスナーの初期化
  initializeEventListeners() {
    // 更新ボタンのイベントリスナー
    const refreshBtn = document.getElementById("refreshBtn");
    if (refreshBtn) {
      refreshBtn.addEventListener("click", () => {
        this.loadUsers();
      });
    }
  }

  // メッセージの処理
  handleMessages() {
    const successAlert = document.querySelector(".alert-success");
    const errorAlert = document.querySelector(".alert-danger");

    if (successAlert) {
      // 3秒後にメッセージを非表示にして一覧を更新
      setTimeout(() => {
        successAlert.style.display = "none";
        this.loadUsers();
      }, 3000);
    }

    if (errorAlert) {
      // エラーメッセージは5秒後に非表示
      setTimeout(() => {
        errorAlert.style.display = "none";
      }, 5000);
    }
  }

  // 認証エラーハンドリング
  handleAuthError(error) {
    if (error.status === 403 && error.response && error.response.requireLogin) {
      CommonUtils.alert(
        "ログインが必要です。未ログインであるためにデータ検索・更新・削除ができません。"
      );
      // ログイン画面にリダイレクト
      window.location.href = "/Login";
      return true;
    }
    return false;
  }

  // ユーザー一覧を取得する関数
  async loadUsers() {
    const loadingMessage = document.getElementById("loadingMessage");
    const errorMessage = document.getElementById("errorMessage");
    const userTable = document.getElementById("userTable");
    const userTableBody = document.getElementById("userTableBody");
    const noDataMessage = document.getElementById("noDataMessage");

    // ローディング表示
    CommonUtils.showLoading();
    CommonUtils.hideError();
    userTable.style.display = "none";
    noDataMessage.style.display = "none";

    try {
      const result = await ApiClient.get("/api/users");

      if (result.success) {
        const users = result.data;

        if (users.length === 0) {
          // データがない場合
          noDataMessage.style.display = "block";
        } else {
          // テーブルにデータを表示
          this.displayUsers(users);
          userTable.style.display = "table";
        }
      } else {
        // エラーメッセージを表示
        CommonUtils.showError(
          result.message || "ユーザー一覧の取得に失敗しました"
        );
      }
    } catch (error) {
      // 認証エラーの場合は特別処理
      if (this.handleAuthError(error)) {
        return;
      }

      // ネットワークエラーなどの場合
      CommonUtils.showError(
        "ネットワークエラーが発生しました: " + error.message
      );
    } finally {
      // ローディング非表示
      CommonUtils.hideLoading();
    }
  }

  // ユーザー一覧を表示する関数
  displayUsers(users) {
    const userTableBody = document.getElementById("userTableBody");
    userTableBody.innerHTML = "";

    users.forEach((user) => {
      const row = document.createElement("tr");
      row.innerHTML = `
                <td>${user.id}</td>
                <td>${CommonUtils.escapeHtml(user.userName)}</td>
                <td>${CommonUtils.escapeHtml(user.email)}</td>
                <td>${user.createdAt}</td>
                <td>
                    <button class="btn btn-danger btn-sm delete-user-btn" data-user-id="${
                      user.id
                    }">削除</button>
                </td>
            `;
      userTableBody.appendChild(row);
    });

    // 削除ボタンにイベントリスナーを追加
    const deleteButtons = document.querySelectorAll(".delete-user-btn");
    deleteButtons.forEach((button) => {
      button.addEventListener("click", (e) => {
        const userId = e.target.getAttribute("data-user-id");
        this.deleteUser(userId);
      });
    });
  }

  // ユーザーを削除する関数
  async deleteUser(userId) {
    if (!(await CommonUtils.confirm("本当にこのユーザーを削除しますか？"))) {
      return;
    }

    try {
      const result = await ApiClient.delete(`/api/users/${userId}`);

      if (result.success) {
        CommonUtils.alert("ユーザーが正常に削除されました");
        // 一覧を再読み込み
        this.loadUsers();
      } else {
        CommonUtils.alert("削除に失敗しました: " + result.message);
      }
    } catch (error) {
      // 認証エラーの場合は特別処理
      if (this.handleAuthError(error)) {
        return;
      }

      CommonUtils.alert("ネットワークエラーが発生しました: " + error.message);
    }
  }
}

// ページ読み込み時の初期化
document.addEventListener("DOMContentLoaded", function () {
  new UserManager();
});
