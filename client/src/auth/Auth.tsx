
import { IoMdArrowRoundBack } from "react-icons/io";
import { TbLoader2 } from "react-icons/tb";
import { useEffect, useState } from "react";
import banner from "../assets/banner_image.jpg"
import BtnLoader from "../components/loader/BtnLoader";
import { apiRequest } from "../services/Api";

export default function Auth() {

  const [activeForm, setActiveForm] = useState<"login" | "register" | "forgot">("login");
  const [isLoading, setIsLoading] = useState<boolean>(false);


  const accessToken = async () => {
    try {
      const response = await apiRequest("/Auth/get_access_token","GET");
      console.log(response);
         setIsLoading(false);
    } catch (error) {
      console.error(error);
         setIsLoading(false);
    }
  }

  const loginUser = async () => {
    try {
      const param = {
          username: "admin",
          password: "123456",
        }
      const response = await apiRequest("/Auth/Login","POST",param);

      console.log(response);
    } catch (error) {
      console.error(error);
    }
  };

  const registerUser = async () => {
    try {
      const param = {
        firstName: "Harshal",
        lastName: "Aglawe",
        userName: "harshaal",
        email: "harshalaglawe1@gmail.com",
        password: "Harshal@06"
      }
      const response = await apiRequest("/Auth/Register","POST",param);

      console.log(response);
    } catch (error) {
      console.error(error);
    }
  };

  useEffect(()=>{
    setIsLoading(true);
    accessToken();
  },[])

  return (
    <>
    {isLoading &&  <div className="h-screen w-screen relative overflow-hidden">
      <TbLoader2
        className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-10 h-10 text-blue-600 animate-spin"
      />
    </div>
    }
    {!isLoading && (<div className="h-screen w-screen overflow-hidden">
      <div className="flex h-screen">
        <div className="hidden lg:block lg:w-[60%] relative">
          <img src={banner} alt="Background" className="w-full h-full object-cover" />

          <div className="absolute inset-0 bg-black/50"></div>

          <div className="absolute inset-0 flex flex-col justify-center px-20 text-white">
            <h1 className="text-6xl font-bold mb-6">Receipe Organizer</h1>

            <p className="text-xl max-w-xl">Your smart kitchen companion for organizing and managing recipes effortlessly.</p>
          </div>
        </div>

        <div className="w-full h-full lg:w-[40%] bg-white px-5 sm:px-8 lg:px-10 py-8 relative overflow-hidden">
          <div className="max-w-xl h-full mx-auto">
            {/* Forms Container */}
            <div className="relative flex flex-col items-center justify-center h-full overflow-hidden">
              {/* ------------------- LOGIN FORM -------------------- */}
              <div
                className={`
                  absolute inset-0 transition-all duration-500 ease-in-out
                  flex flex-col items-center justify-center px-4
                  ${activeForm === "login" ? "translate-x-0 opacity-100" : "-translate-x-full opacity-0 pointer-events-none"}
                `}
                aria-hidden={activeForm !== "login"}
              >
                <div className="w-full px-4">
                  <div className="mb-8">
                    <h2 className="text-3xl font-bold text-slate-700">Welcome Back</h2>
                    <p className="text-slate-500 mt-2">Sign in to access your account.</p>
                  </div>

                  <form className="space-y-5">
                    <div>
                      <label>Email Address / Username</label>
                      <input type="text" placeholder="Enter your email or username" tabIndex={activeForm === "login" ? 0 : -1} />
                    </div>

                    <div>
                      <label >Password</label>
                      <input type="password" placeholder="Enter your password" tabIndex={activeForm === "login" ? 0 : -1} />
                    </div>

                    <div className="flex justify-end">
                      <button type="button" className="cursor-pointer text-blue-600 font-semibold" onClick={() => setActiveForm("forgot")} tabIndex={activeForm === "login" ? 0 : -1}>
                        Forgot Password?
                      </button>
                    </div>

                    <button onClick={loginUser} type="button" className="cursor-pointer w-full h-11 sm:h-12 bg-blue-600 hover:bg-blue-700 text-white rounded-xl font-semibold flex items-center justify-center gap-2" tabIndex={activeForm === "login" ? 0 : -1}>
                      <BtnLoader/>
                      Login
                    </button>
                  </form>

                  <div className="text-center mt-8">
                    <span className="text-gray-500">Don't have an account?</span>
                    <button type="button" onClick={() => setActiveForm("register")} className="cursor-pointer ml-1 text-blue-600 font-semibold" tabIndex={activeForm === "login" ? 0 : -1}>
                      Register Here
                    </button>
                  </div>
                </div>
              </div>

              {/* ----------------- REGISTER FORM -------------------- */}
              <div
                className={`
                  absolute inset-0 transition-all duration-500 ease-in-out
                  flex flex-col items-center justify-start 2xl:justify-center
                  ${activeForm === "register" ? "translate-x-0 opacity-100" : activeForm === "login" ? "translate-x-full opacity-0 pointer-events-none" : "-translate-x-full opacity-0 pointer-events-none"}
                `}
                aria-hidden={activeForm !== "register"}
              >
                <div className="w-full px-4 overflow-y-auto">
                  <div className="mb-8">
                    <h2 className="text-3xl font-bold text-slate-700">Create Account</h2>
                    <p className="text-slate-500 mt-2">Create your account to get started.</p>
                  </div>

                  <form className="space-y-4">
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                      <div>
                        <label >First Name</label>
                        <input type="text" placeholder="Enter first name" tabIndex={activeForm === "register" ? 0 : -1} />
                      </div>
                      <div>
                        <label >Last Name</label>
                        <input type="text" placeholder="Enter last name" tabIndex={activeForm === "register" ? 0 : -1} />
                      </div>
                    </div>

                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                      <div>
                        <label >Username</label>
                        <input type="text" placeholder="Enter username" tabIndex={activeForm === "register" ? 0 : -1} />
                      </div>
                      <div>
                        <label >Email Address</label>
                        <input type="email" placeholder="Enter email address" tabIndex={activeForm === "register" ? 0 : -1} />
                      </div>
                    </div>

                    <div>
                      <label >Password</label>
                      <input type="password" placeholder="Enter password" tabIndex={activeForm === "register" ? 0 : -1} />
                    </div>

                    <div>
                      <label >Confirm Password</label>
                      <input type="password" placeholder="Confirm password" tabIndex={activeForm === "register" ? 0 : -1} />
                    </div>

                    <button onClick={registerUser} type="button" className="cursor-pointer w-full h-11 sm:h-12 bg-blue-600 hover:bg-blue-700 text-white rounded-xl font-semibold flex items-center justify-center gap-2" tabIndex={activeForm === "login" ? 0 : -1}>
                      <BtnLoader/>
                      Create Account
                    </button>
                  </form>

                  <div className="text-center mt-8">
                    <span className="text-gray-500">Already have an account?</span>
                    <button type="button" onClick={() => setActiveForm("login")} className="cursor-pointer ml-1 text-blue-600 font-semibold" tabIndex={activeForm === "register" ? 0 : -1}>
                      Login Here
                    </button>
                  </div>
                </div>
              </div>

              {/* ---------------- FORGOT PASSWORD FORM --------------- */}
              <div
                className={`
                  absolute inset-0 transition-all duration-500 ease-in-out
                  flex flex-col items-center justify-center
                  ${activeForm === "forgot" ? "translate-x-0 opacity-100" : "translate-x-full opacity-0 pointer-events-none"}
                `}
                aria-hidden={activeForm !== "forgot"}
              >
                <div className="w-full px-4">
                  <div className="mb-8 flex items-center gap-5">
                    <button type="button" onClick={() => setActiveForm("login")} className="cursor-pointer h-10 w-10 rounded-xl border border-gray-300 flex items-center justify-center hover:bg-gray-100" tabIndex={activeForm === "forgot" ? 0 : -1}>
                      <IoMdArrowRoundBack className="text-xl" />
                    </button>
                    <div>
                      <h2 className="text-3xl font-bold text-slate-700">Forgot Password</h2>
                      <p className="text-slate-500 mt-2">Enter your username or email to reset your password.</p>
                    </div>
                  </div>

                  <form className="space-y-5">
                    <div>
                      <label >Email Address / Username</label>
                      <input type="text" placeholder="Enter your email or username" tabIndex={activeForm === "forgot" ? 0 : -1} />
                    </div>

                    <button type="button" className="cursor-pointer w-full h-11 sm:h-12 bg-blue-600 hover:bg-blue-700 text-white rounded-xl font-semibold flex items-center justify-center gap-2" tabIndex={activeForm === "login" ? 0 : -1}>
                      <BtnLoader/>
                      Send Reset Link
                    </button>
                  </form>
                </div>
              </div>
            </div>
          </div>
          <div className="absolute bottom-4 text-slate-400 text-center w-full left-0">Powered by Axis</div>
        </div>
      </div>
    </div>)}
    </>
  );
}
