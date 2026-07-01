import { useEffect, useState } from "react";
import { Outlet } from "react-router-dom";
import { apiRequest } from "./services/Api";
import { TbLoader2 } from "react-icons/tb";
import { useNavigate } from "react-router-dom";

function App() {
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const navigate = useNavigate();

  const accessToken = async () => {
    try {
      const response = await apiRequest("/Auth/get_access_token", "GET");
      console.log(response);
      navigate("/home", { replace: true });
    } catch (error) {
      console.error(error);
      navigate("/auth", { replace: true });
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    accessToken();
  }, []);

  return (
    <>
      {isLoading && (
        <div className="h-screen w-screen relative overflow-hidden">
          <TbLoader2 className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-10 h-10 text-blue-600 animate-spin" />
        </div>
      )}
      {!isLoading && <><Outlet /></>}
    </>
  );
}

export default App;
