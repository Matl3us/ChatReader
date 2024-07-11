const Login = () => {
  const client_id = import.meta.env.VITE_CLIENT_ID;
  const redirect_uri = "http://localhost:5240/api/auth/";
  const scope = "chat:edit+chat:read";

  return (
    <div className="p-16">
      <h1 className="text-3xl font-semibold text-text mb-6">Login to chat reader app</h1>
      <button className="bg-button rounded-md p-2">
        <a
          href={`https://id.twitch.tv/oauth2/authorize?client_id=${client_id}&redirect_uri=${redirect_uri}&response_type=code&scope=${scope}`}
        >
          <span className="font-semibold text-text"> Connect with Twitch</span>
        </a>
      </button>
    </div>
  );
};

export default Login;
