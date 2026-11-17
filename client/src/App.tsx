import {createBrowserRouter, type RouteObject, RouterProvider} from "react-router-dom";
import {LoginPage} from "./pages/LoginPage.tsx";

const routes: RouteObject[] = [
    {
        path:"/login",
        element: <LoginPage/>
    },
];

function App() {
    return <RouterProvider router={createBrowserRouter(routes)} />;
}

export default App;