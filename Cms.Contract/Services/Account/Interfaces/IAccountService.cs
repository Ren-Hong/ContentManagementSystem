using Cms.Contract.Common.Pagination;
using Cms.Contract.Services.Account.Dtos;

namespace Cms.Contract.Services.Account.Interfaces
{
    /// <summary>
    /// 提供帳號登入與帳號管理相關的應用服務操作。
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// 驗證帳號密碼並建立登入結果。
        /// </summary>
        /// <param name="req">登入請求資料。</param>
        /// <returns>登入處理結果。</returns>
        Task<LoginResponseDto> LoginAsync(LoginRequestDto req);

        /// <summary>
        /// 以分頁方式取得帳號摘要資料。
        /// </summary>
        /// <param name="pageRequest">分頁條件。</param>
        /// <returns>帳號摘要的分頁結果。</returns>
        Task<PagedResult<AccountListResponseDto>> PageAccountsAsync(PageRequest pageRequest);

        /// <summary>
        /// 建立帳號。
        /// </summary>
        /// <param name="req">建立帳號請求資料。</param>
        /// <returns>建立帳號結果。</returns>
        Task<CreateAccountResponseDto> CreateAccountAsync(CreateAccountRequestDto req);

        /// <summary>
        /// 更新帳號資料。
        /// </summary>
        /// <param name="req">更新帳號請求資料。</param>
        /// <returns>更新帳號結果。</returns>
        Task<UpdateAccountResponseDto> UpdateAccountAsync(UpdateAccountRequestDto req);

        /// <summary>
        /// 重設帳號密碼。
        /// </summary>
        /// <param name="req">重設密碼請求資料。</param>
        /// <returns>重設密碼結果。</returns>
        Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordRequestDto req);

        /// <summary>
        /// 刪除帳號。
        /// </summary>
        /// <param name="req">刪除帳號請求資料。</param>
        /// <returns>刪除帳號結果。</returns>
        Task<DeleteAccountResponseDto> DeleteAccountAsync(DeleteAccountRequestDto req);
    }
}
