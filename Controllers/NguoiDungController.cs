using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using CLEANSHOP.Models;
using Facebook;

namespace CLEANSHOP.Controllers
{
    public class NguoiDungController : Controller
    {
        MydataDataContext data = new MydataDataContext();
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangky(FormCollection collection, Customer kh)
        {


            var Name = collection["Name"];
            var LoginName = collection["LoginName"];
            var Password = collection["Password"];
            var ConfirmPassword = collection["ConfirmPassword"];
            var Email = collection["Email"];
            var Address = collection["Address"];
            var Phone = collection["Phone"];
            var limit = Convert.ToInt32(collection["limit"]);
            var DateofBirth = String.Format("{0:MM/dd/yyyy}", collection["DateofBirth"]);



            if (String.IsNullOrEmpty(ConfirmPassword))
                ViewData["NhapMKXN"] = "Phải nhập đủ thông tin!";
            else if (String.IsNullOrEmpty(Name))
                ViewData["NhapHoten"] = "Phải nhập đủ họ tên";
            else if (String.IsNullOrEmpty(Phone))
                ViewData["nhapDT"] = "Phải nhập số điện thoại";
            else if (String.IsNullOrEmpty(Address))
                ViewData["nhapDC"] = "Phải nhập địa chỉ";
            else if (String.IsNullOrEmpty(Email))
                ViewData["nhapEmail"] = "Phải nhập Email";
            else if (String.IsNullOrEmpty(LoginName))
                ViewData["NhapTK"] = "Phải nhập tên đăng nhập";
            else if (String.IsNullOrEmpty(Password))
                ViewData["nhapMK"] = "Phải nhập mật khẩu";
            else if (Password.Length > 8)
                ViewData["dodaiMK"] = "mật khẩu phải nhiều hơn 8 ký tự";

            else if (String.IsNullOrEmpty(Password))
                ViewData["nhapMK"] = "Phải nhập mật khẩu";
            else if (String.IsNullOrEmpty(DateofBirth))
                ViewData["nhapNS"] = "Phải nhập ngày sinh";
            else
            {
                if (!Password.Equals(ConfirmPassword))
                {
                    ViewData["MatKhauGiongNhau"] = "Mật khẩu và mật khẩu xác nhận phải giống nhau";
                }
                else
                {
                    kh.Name = Name;
                    kh.LoginName = LoginName;
                    kh.Password = Password;
                    kh.Email = Email;
                    kh.Address = Address;
                    kh.Phone = Phone;
                    kh.limit = limit;
                    kh.DateofBirth = DateTime.Parse(DateofBirth);
                    data.Customers.InsertOnSubmit(kh);
                    data.SubmitChanges();
                    return RedirectToAction("DangNhap");
                }
            }
            return this.DangKy();
        }












        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var LoginName = collection["LoginName"];
            var Password = collection["Password"];
            var limit = Convert.ToInt32(collection["limit"]);
            Customer kh = data.Customers.SingleOrDefault(n => n.LoginName == LoginName && n.Password == Password && n.limit >= 0);
            if (kh != null)
            {
                ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                Session["Taikhoan"] = kh;
                return RedirectToAction("Index", "DichVu");
            }
            else
            {
                ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
           
            return View();
        }
       


        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("DangNhap");
        }


        public JsonResult CheckUsername(string userdata)
        {
            System.Threading.Thread.Sleep(200);
            var SearchData = data.Customers.Where(x => x.LoginName == userdata).SingleOrDefault();

            if (SearchData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        public JsonResult CheckEmail(string userdata)
        {
            System.Threading.Thread.Sleep(200);
            var SearchData = data.Customers.Where(x => x.Email == userdata).SingleOrDefault();

            if (SearchData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

       public JsonResult CheckPhone(string userdata)
        {
            System.Threading.Thread.Sleep(200);
            var SearchData = data.Customers.Where(x => x.Phone == userdata).SingleOrDefault();

            if (SearchData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        private double TongTien()
        {
            double tt = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                tt = lstGiohang.Sum(n => n.TotalPrice);
            }

            if (tt <= 300000)
            {
                return tt;
            }
            else
            {
                if (tt > 300000 && tt <= 500000)
                {
                    return tt * 0.7;
                }
                else
                {
                    return tt * 0.5;
                }
            }
        }


        public ActionResult sendPass(string pass, System.Web.Mvc.FormCollection collection)

        {
           Cart dh = new Cart();
           Customer kh = (Customer)Session["Taikhoan"];

          /*  var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);*/
            dh.BookingDate = DateTime.Now;
           

            MailAddress fromAddress = new MailAddress("binhminh220901@gmail.com", "PHỤ KIỆN GIÀY AK2M");

            MailAddress toAddress = new MailAddress(kh.Email.ToString());

            const string fromPassword = "ipzzdyzcoriefohw";

            string subject = "Xác nhận đơn hàng";

            string Tien = TongTien().ToString();

            string Soluong = TongSoLuongSanPham().ToString();

            string Ten = kh.Name.ToString();

            string NgayDat = dh.BookingDate.ToString();

            string madh = dh.IdCart.ToString();


            SmtpClient smtp = new SmtpClient

            {

                Host = "smtp.gmail.com",

                Port = 587,

                EnableSsl = true,

                DeliveryMethod = SmtpDeliveryMethod.Network,

                UseDefaultCredentials = false,

                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)

            };

            using (MailMessage message = new MailMessage(fromAddress, toAddress)

            {

                Subject = subject,

                Body =

                        "<p>Xác nhận đơn hàng gồm : </p>" +
                         "<p>Số lượng sản phẩm :  " + Soluong + " sản phẩm  </p>" +
                         "<p>Tên KH : " + Ten + "   </p>" +
                          "<p>Email : " + kh.Email.ToString() + "   </p>" +
                         "<p color = red>Tổng thành tiền : " + Tien + " VND  </p>"
                        + "<p>Ngày đặt hàng  :" + NgayDat + "</p>"
                        + "<p>Mã đơn hàng  : " + madh + "</p>" +
                        "<button> Xác nhận đơn hàng </Button>",

                IsBodyHtml = true,

            })

            {

                smtp.Send(message);
                return RedirectToAction("DatHang","GioHang");
            }
            
        }



       
        private int TongSoLuongSanPham()
        {
            int tsl = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                tsl = lstGiohang.Count;
            }
            return tsl;
        }
        
        public ActionResult profile(int id)
        {
            Customer kh = (Customer)Session["Taikhoan"];
            
            var E_Pro = data.Customers.First(m => m.IdCustomer == id);
            return View(E_Pro);
        }
        [HttpPost]
        public ActionResult profile(int id, FormCollection collection)
        {
            Customer kh = (Customer)Session["Taikhoan"];

            var E_Pro = data.Customers.First(m => m.IdCustomer == id);

            var hoten = collection["Name"];
            var tendangnhap = collection["LoginName"];
            var matkhau = collection["Password"];
            var MatkhauXacNhan = collection["MatKhauXacNhan"];
            var email = collection["Email"];
            var diachi = collection["Address"];
            var dienthoai = collection["Phone"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["DateofBirth"]);

           
            if (string.IsNullOrEmpty(hoten))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {

                kh.LoginName = tendangnhap;
                kh.Password = matkhau;
                kh.Email = email;
                kh.Address = diachi;
                kh.Phone = dienthoai;
                kh.DateofBirth = DateTime.Parse(ngaysinh);
                kh.IdCustomer = Convert.ToInt32(collection["id"]);

                UpdateModel(kh);
                data.SubmitChanges();
                return RedirectToAction("Index", "DichVu");
            }
            return this.profile(id);
        }

        public ActionResult DonHangCuaToi(int id)
        {    
            List<Cart> dh = data.Carts.Where(m => m.Customer_Id == id).ToList();
            var tt = data.Carts.Where(m => m.Customer_Id == id ).Sum(n => Convert.ToInt32(n.TotalPrice));
            ViewBag.tongtien2 = tt;
            return View(dh);
        }
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }
        public ActionResult LoginFB()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });

            return Redirect(loginUrl.AbsoluteUri);
        }
        public ActionResult FacebookCallback( string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });


            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                // Get the user's information, like email, first name, middle name etc
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string email = me.email;
                string userName = me.email;
                string firstname = me.first_name;
                string middlename = me.middle_name;
                string lastname = me.last_name;
              
                Customer user = new Customer(); 
                Session["Taikhoan"] = user;      
                //user.Email = "no";
                //user.LoginName = "no";
                user.Name = firstname + " " + middlename + " " + lastname;
                user.limit = 0;
                //user.Password = "no";
                //user.Address = "no";
                //user.Phone = "no";
                user.DateofBirth = DateTime.Now;
                data.Customers.InsertOnSubmit(user);
                data.SubmitChanges();
                return Redirect("/");
            }
            return Redirect("/");

        }

    }


}
