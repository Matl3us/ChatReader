const Login = () => {
  const client_id = import.meta.env.VITE_CLIENT_ID;
  const redirect_uri = "http://localhost:5240/api/auth/";

  return (
    <div>
      <a
        href={`https://id.twitch.tv/oauth2/authorize?client_id=${client_id}&redirect_uri=${redirect_uri}&response_type=code&scope=`}
      >
        Connect with Twitch
      </a>
    </div>
  );
};

export default Login;
