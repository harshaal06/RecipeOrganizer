import axios from "axios";
import type { AxiosRequestConfig } from "axios";

const api = axios.create({
  baseURL: "http://localhost:5157/api/v1",
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 10000,
  withCredentials: true,
});

export type RequestType = "GET" | "POST" | "PUT" | "PATCH" | "DELETE";

export const apiRequest = async <T = any>(
  req_url: string,
  req_type: RequestType,
  parameters?: Record<string, any>,
): Promise<T> => {
  try {
    const config: AxiosRequestConfig = {
      url: req_url,
      method: req_type,
    };

    if (req_type !== "GET" && parameters) {
      config.data = parameters;
    }
    const response = await api(config);
    return response.data;
  } catch (error: any) {
    console.error("API Error:", error);
    throw error?.response?.data || error;
  }
};
