const path = require('path');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const rtlcss = require('rtlcss');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const fs = require('fs-extra');
const { RawSource } = require('webpack-sources');
const BrowserSyncPlugin = require('browser-sync-webpack-plugin');
// Define asset folders (adjust as needed for your PHP structure)
const folder = {
    src: "./src/",
    src_assets: "./src/assets/",
    dist: "./wwwroot/", // PHP apps often use public/ or webroot
    dist_assets: "./wwwroot/assets/"
};
// CSS files to generate RTL variants for
const cssPairs = [
    { ltr: 'css/app.min.css', rtl: 'css/app-rtl.min.css' },
    { ltr: 'css/bootstrap.min.css', rtl: 'css/bootstrap-rtl.min.css' },
];
module.exports = {
    mode: 'development', // or 'production'
    entry: {
        app: path.join(__dirname, folder.src_assets, 'scss/app.scss'),
        bootstrap: path.join(__dirname, folder.src_assets, 'scss/bootstrap.scss'),
        icons: path.join(__dirname, folder.src_assets, 'scss/icons.scss'),
    },
    output: {
        path: path.resolve(__dirname, folder.dist_assets),
        filename: 'chunk/[name].js',
    },
    performance: {
        hints: false,
    },
    devServer: {
        static: {
            directory: path.join(__dirname, folder.dist),
            serveIndex: true,
        },
        hot: "only",
    },
    module: {
        rules: [
            {
                test: /\.scss$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    'css-loader',
                    'sass-loader',
                ],
            },
        ],
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: 'css/[name].min.css',
        }),
        new CopyWebpackPlugin({
            patterns: [
                { from: path.join(__dirname, folder.src_assets, 'images'), to: path.join(__dirname, folder.dist_assets, 'images') },
                { from: path.join(__dirname, folder.src_assets, 'js'), to: path.join(__dirname, folder.dist_assets, 'js') },
                { from: path.join(__dirname, folder.src_assets, 'fonts'), to: path.join(__dirname, folder.dist_assets, 'fonts') },
                { from: path.join(__dirname, folder.src_assets, 'libs'), to: path.join(__dirname, folder.dist_assets, 'libs') },
            ],
        }),
        new BrowserSyncPlugin({
            proxy: 'http://localhost:8000', // Replace with your local PHP dev server (e.g., Laravel, XAMPP)
            files: [
                '**/*.php', // Watch PHP files
                path.join(folder.dist_assets, '**/*.css'),
                path.join(folder.dist_assets, '**/*.js'),
            ],
            open: false,
        }),
        {
            apply(compiler) {
                compiler.hooks.thisCompilation.tap('GenerateRTL', (compilation) => {
                    compilation.hooks.processAssets.tap(
                        {
                            name: 'GenerateRTL',
                            stage: compilation.PROCESS_ASSETS_STAGE_ADDITIONAL,
                        },
                        () => {
                            cssPairs.forEach((pair) => {
                                const ltrAsset = compilation.assets[pair.ltr];
                                if (ltrAsset) {
                                    const ltrCss = ltrAsset.source();
                                    const rtlCss = rtlcss.process(ltrCss, { autoRename: false, clean: false });
                                    compilation.emitAsset(pair.rtl, new RawSource(rtlCss));
                                }
                            });
                        }
                    );
                });
            },
        },
    ],
};