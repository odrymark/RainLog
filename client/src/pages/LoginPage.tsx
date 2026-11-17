import React, { useState, useEffect } from 'react';

type LoginFormData = {
    username: string;
    password: string;
    companyShortName: string;
};

export function LoginPage() {
    const [formData, setFormData] = useState<LoginFormData>({
        username: '',
        password: '',
        companyShortName: '',
    });
    const [showPassword, setShowPassword] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (error) {
            setError(null);
        }
    }, [formData]);

    useEffect(() => {
        if (!isLoading) return;

        let isCancelled = false;

        const performLogin = async () => {
            try {
                await new Promise((resolve) => setTimeout(resolve, 800));
                if (isCancelled) return;

                console.log('Login submitted:', formData);
            } catch (err) {
                if (!isCancelled) {
                    setError(err instanceof Error ? err.message : 'Invalid login credentials.');
                }
            } finally {
                if (!isCancelled) {
                    setIsLoading(false);
                }
            }
        };

        performLogin();

        return () => {
            isCancelled = true;
        };
    }, [isLoading]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData((prev) => ({
            ...prev,
            [name]: name === 'companyShortName' ? value.trim().toLowerCase() : value,
        }));
    };

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError(null);

        if (!formData.username || !formData.password || !formData.companyShortName) {
            setError('Please fill in all fields.');
            return;
        }

        setIsLoading(true);
    };

    return (
        <div className="min-h-screen bg-base-200 flex flex-col justify-between p-6">
            <header className="w-full max-w-5xl mx-auto flex items-center justify-between">
                <div className="flex items-center gap-2 font-bold text-lg tracking-tight">
                    <span className="w-2.5 h-2.5 rounded-full bg-primary inline-block"/>
                    <span>RainLog</span>
                </div>
            </header>

            <main className="w-full max-w-sm mx-auto my-auto py-8">
                <div className="bg-base-100 rounded-box border border-base-300 overflow-hidden shadow-sm">
                    <div className="h-1.5 w-full bg-primary"/>

                    <div className="p-8 space-y-6">
                        <div className="space-y-1">
                            <h1 className="text-2xl font-bold tracking-tight">Sign in</h1>
                            <p className="text-sm text-base-content/60">
                                Enter your credentials to continue.
                            </p>
                        </div>

                        {error && (
                            <div role="alert" className="alert alert-error alert-outline text-xs py-2.5">
                                <span>{error}</span>
                            </div>
                        )}

                        <form onSubmit={handleSubmit} className="space-y-4">
                            <label className="floating-label w-full">
                                <span>Username</span>
                                <input
                                    type="text"
                                    name="username"
                                    placeholder="Username"
                                    className="input input-bordered input-md w-full"
                                    value={formData.username}
                                    onChange={handleChange}
                                    autoComplete="username"
                                    required
                                />
                            </label>

                            <div className="space-y-1.5">
                                <div className="relative">
                                    <label className="floating-label w-full">
                                        <span>Password</span>
                                        <input
                                            type={showPassword ? 'text' : 'password'}
                                            name="password"
                                            placeholder="Password"
                                            className="input input-bordered input-md w-full pr-14"
                                            value={formData.password}
                                            onChange={handleChange}
                                            autoComplete="current-password"
                                            required
                                        />
                                    </label>
                                    <button
                                        type="button"
                                        onClick={() => setShowPassword(!showPassword)}
                                        className="btn btn-ghost btn-xs absolute right-2 top-1/2 -translate-y-1/2 text-base-content/60"
                                    >
                                        {showPassword ? 'Hide' : 'Show'}
                                    </button>
                                </div>
                                <div className="flex justify-end">
                                    <a
                                        href="#forgot-password"
                                        className="link link-hover text-xs text-base-content/60 hover:text-primary"
                                    >
                                        Forgot password?
                                    </a>
                                </div>
                            </div>

                            <label className="floating-label w-full">
                                <span>Company Short Name</span>
                                <input
                                    type="text"
                                    name="companyShortName"
                                    placeholder="Company Short Name"
                                    className="input input-bordered input-md w-full font-mono text-sm"
                                    value={formData.companyShortName}
                                    onChange={handleChange}
                                    autoComplete="organization"
                                    required
                                />
                            </label>

                            <button
                                type="submit"
                                disabled={isLoading}
                                className="btn btn-neutral w-full mt-2"
                            >
                                {isLoading ? (
                                    <>
                                        <span className="loading loading-dots loading-sm"/>
                                        Signing in
                                    </>
                                ) : (
                                    'Sign in'
                                )}
                            </button>
                        </form>
                    </div>
                </div>
            </main>

            <footer className="w-full max-w-5xl mx-auto text-center text-xs text-base-content/40">
                &copy; {new Date().getFullYear()} RainLog
            </footer>
        </div>
    );
}