<manifest xmlns:android="http://schemas.android.com/apk/res/android"
    package="com.yourcompany.googlelogin">

    <!-- 인터넷 권한 필수 -->
    <uses-permission android:name="android.permission.INTERNET" />

    <application
        android:label="@string/app_name"
        android:icon="@mipmap/ic_launcher">

        <!-- 우리가 만든 MainActivity 등록 -->
        <activity
            android:name="com.Avengers.projectNemesis.MainActivity"
            android:launchMode="singleTask"
            android:theme="@style/UnityThemeSelector"
            android:exported="true">
            
            <intent-filter>
                <action android:name="android.intent.action.MAIN" />
                <category android:name="android.intent.category.LAUNCHER" />
            </intent-filter>
        </activity>

    </application>
</manifest>
