using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Web;
using FurnitureShop.Areas.VNPayAPI.Util;
using FurnitureShop.Models;
using System;

namespace FurnitureShop.Areas.VNPayAPI.Controllers
{
    [Area("VNPayAPI")]
    public class HomeController : Controller
    {
        private readonly VNPaySettings _vnpaySettings;

        public HomeController(IOptions<VNPaySettings> vnpaySettings)
        {
            _vnpaySettings = vnpaySettings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Thực hiện thanh toán qua VNPAY
        [Route("/VNPayAPI/Payment/{amount}&{infor}&{orderRef}")]
        public IActionResult Payment(string amount, string infor, string orderRef)
        {
            // Lấy địa chỉ IP của khách hàng
            string clientIPAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();

            // Khởi tạo đối tượng PayLib để xử lý yêu cầu thanh toán
            PayLib pay = new PayLib();

            // Thêm dữ liệu vào yêu cầu
            pay.AddRequestData("vnp_Version", "2.1.0");
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", _vnpaySettings.TmnCode);
            pay.AddRequestData("vnp_Amount", (Convert.ToDecimal(amount) * 100).ToString()); // Số tiền phải nhân với 100
            pay.AddRequestData("vnp_BankCode", "");
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", clientIPAddress); // Địa chỉ IP của khách hàng
            pay.AddRequestData("vnp_Locale", "vn");
            pay.AddRequestData("vnp_OrderInfo", infor);
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", _vnpaySettings.ReturnUrl);
            pay.AddRequestData("vnp_TxnRef", orderRef);

            // Tạo URL thanh toán
            string paymentUrl = pay.CreateRequestUrl(_vnpaySettings.Url, _vnpaySettings.HashSecret);

            // In ra URL thanh toán để kiểm tra
            Console.WriteLine(paymentUrl); // Đây là nơi bạn in URL thanh toán

            // Redirect đến VNPAY
            return Redirect(paymentUrl);
        }




        // Xử lý kết quả trả về từ VNPAY
        [Route("/VNPayAPI/paymentconfirm")]
        public IActionResult PaymentConfirm()
        {
            if (Request.QueryString.HasValue)
            {
                var queryString = Request.QueryString.Value;
                var json = HttpUtility.ParseQueryString(queryString);

                // Lấy các tham số trả về từ VNPAY
                long orderId = Convert.ToInt64(json["vnp_TxnRef"]);
                string orderInfo = json["vnp_OrderInfo"];
                long vnpayTranId = Convert.ToInt64(json["vnp_TransactionNo"]);
                string vnp_ResponseCode = json["vnp_ResponseCode"];
                string vnp_SecureHash = json["vnp_SecureHash"];
                var pos = queryString.IndexOf("&vnp_SecureHash");

                // Kiểm tra chữ ký bảo mật
                bool checkSignature = ValidateSignature(queryString.Substring(1, pos - 1), vnp_SecureHash, _vnpaySettings.HashSecret);
                if (checkSignature && _vnpaySettings.TmnCode == json["vnp_TmnCode"])
                {
                    if (vnp_ResponseCode == "00")
                    {

                        // Thanh toán thành công
                        // Cập nhật trạng thái đơn hàng trong hệ thống (nếu cần)
                        return Redirect("/Cart/Thankyou");
                    }
                    else
                    {
                        // Thanh toán không thành công
                        TempData["ErrorMessage"] = "Payment failed!";
                        return Redirect("/Cart/Checkout");
                    }
                }
                else
                {
                    // Phản hồi không khớp với chữ ký bảo mật
                    TempData["ErrorMessage"] = "Invalid signature!";
                    return Redirect("/Cart/Checkout");
                }
            }
            // Phản hồi không hợp lệ
            TempData["ErrorMessage"] = "Invalid response from payment gateway.";
            return Redirect("/Cart/Checkout");
        }

        // Phương thức kiểm tra chữ ký bảo mật
        private bool ValidateSignature(string rspraw, string inputHash, string secretKey)
        {
            string myChecksum = PayLib.HmacSHA512(secretKey, rspraw); // Tính toán chữ ký bảo mật của bạn
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
