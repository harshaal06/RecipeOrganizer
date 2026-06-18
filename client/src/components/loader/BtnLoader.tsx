import { TbLoader } from "react-icons/tb";

function BtnLoader({ loading }: { loading: boolean }) {
  return (
    loading && <TbLoader className="w-4 h-4 mt-0.5 animate-spin" />
  );
}

export default BtnLoader;